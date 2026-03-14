using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CarSimulator.Multiplayer
{
    public class SimpleServer
    {
        private static List<TcpClient> m_clients = new List<TcpClient>();
        private static int m_nextPlayerId = 1;
        private static Dictionary<int, string> m_playerNames = new Dictionary<int, string>();

        public static void StartServer(int port)
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            Console.WriteLine($"[Server] Started on port {port}");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                lock (m_clients)
                {
                    int playerId = m_nextPlayerId++;
                    m_clients.Add(client);
                    m_playerNames[playerId] = $"Player{playerId}";

                    Console.WriteLine($"[Server] Player {playerId} connected");

                    Thread clientThread = new Thread(() => HandleClient(client, playerId));
                    clientThread.Start();
                }
            }
        }

        private static void HandleClient(TcpClient client, int playerId)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];

            Broadcast($"PLAYER_JOIN|{playerId}|{m_playerNames[playerId]}", playerId);

            string existingPlayers = "PLAYER_LIST";
            lock (m_clients)
            {
                foreach (var existing in m_playerNames)
                {
                    if (existing.Key != playerId)
                    {
                        existingPlayers += $"|{existing.Key}|{existing.Value}";
                    }
                }
            }
            SendToClient(client, existingPlayers);

            SendToClient(client, $"WELCOME|{playerId}");

            while (client.Connected)
            {
                try
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            ProcessMessage(data, playerId);
                        }
                    }
                    Thread.Sleep(10);
                }
                catch
                {
                    break;
                }
            }

            lock (m_clients)
            {
                m_clients.Remove(client);
                m_playerNames.Remove(playerId);
            }

            Broadcast($"PLAYER_LEAVE|{playerId}", playerId);
            client.Close();
            Console.WriteLine($"[Server] Player {playerId} disconnected");
        }

        private static void ProcessMessage(string data, int playerId)
        {
            string[] parts = data.Split('|');

            switch (parts[0])
            {
                case "JOIN":
                    if (parts.Length > 2)
                    {
                        lock (m_playerNames)
                        {
                            m_playerNames[playerId] = parts[2];
                        }
                        Console.WriteLine($"[Server] Player {playerId} named: {parts[2]}");
                    }
                    break;

                case "UPDATE":
                    Broadcast(data, playerId);
                    break;

                case "CHAT":
                    Console.WriteLine($"[Chat] {m_playerNames[playerId]}: {parts[1]}");
                    Broadcast($"CHAT|{m_playerNames[playerId]}|{parts[1]}", -1);
                    break;
            }
        }

        private static void SendToClient(TcpClient client, string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                client.GetStream().Write(data, 0, data.Length);
            }
            catch { }
        }

        private static void Broadcast(string message, int excludePlayerId)
        {
            lock (m_clients)
            {
                for (int i = m_clients.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        SendToClient(m_clients[i], message);
                    }
                    catch
                    {
                        m_clients.RemoveAt(i);
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            int port = args.Length > 0 ? int.Parse(args[0]) : 8888;
            StartServer(port);
        }
    }
}
