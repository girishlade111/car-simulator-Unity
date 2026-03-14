using UnityEngine;

namespace CarSimulator.Config
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "CarSimulator/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Scene Names")]
        public string defaultScene = "MainMenu";
        public string openWorldScene = "OpenWorld_TestDistrict";
        public string garageScene = "Garage_Test";

        [Header("Game Settings")]
        public bool enableAutoSave = true;
        public float autoSaveInterval = 120f;
        public int maxSaveSlots = 10;

        [Header("Debug")]
        public bool enableDebugLogging = true;
        public bool showDebugScreen = false;

        [Header("Performance")]
        public int targetFrameRate = 60;
        public float fixedDeltaTime = 0.02f;
    }
}
