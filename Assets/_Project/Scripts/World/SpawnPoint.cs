using UnityEngine;

namespace CarSimulator.World
{
    public class SpawnPoint : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private bool m_isDefaultSpawn;
        [SerializeField] private string m_spawnPointName = "SpawnPoint";

        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public string SpawnName => m_spawnPointName;
        public bool IsDefault => m_isDefaultSpawn;

        private void Awake()
        {
            if (m_isDefaultSpawn)
            {
                SpawnManager.Instance?.RegisterDefaultSpawn(this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = m_isDefaultSpawn ? Color.green : Color.blue;
            Gizmos.DrawWireSphere(transform.position, 1f);
            
            Vector3 direction = transform.forward * 2f;
            Gizmos.DrawLine(transform.position, transform.position + direction);
            
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.5f);
            Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "Prefab Icon", true);
        }
    }
}
