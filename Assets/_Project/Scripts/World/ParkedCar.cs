using UnityEngine;

namespace CarSimulator.World
{
    public class ParkedCar : MonoBehaviour
    {
        [Header("Car Data")]
        [SerializeField] private string m_carId;
        [SerializeField] private ParkedCarType m_carType = ParkedCarType.Sedan;
        [SerializeField] private Color m_bodyColor = Color.white;

        [Header("State")]
        [SerializeField] private bool m_isLocked = true;
        [SerializeField] private float m_damageLevel = 0f;

        [Header("References")]
        [SerializeField] private ParkedCarSpawnPoint m_spawnPoint;

        private Renderer[] m_renderers;

        public enum ParkedCarType
        {
            Sedan,
            SUV,
            Truck,
            Sports,
            Compact,
            Van,
            Motorcycle
        }

        private void Awake()
        {
            m_renderers = GetComponentsInChildren<Renderer>();
            m_carId = System.Guid.NewGuid().ToString();
        }

        private void Start()
        {
            ApplyColor();
        }

        public void SetSpawnPoint(ParkedCarSpawnPoint spawnPoint)
        {
            m_spawnPoint = spawnPoint;
        }

        public void SetBodyColor(Color color)
        {
            m_bodyColor = color;
            ApplyColor();
        }

        private void ApplyColor()
        {
            if (m_renderers == null) return;

            foreach (var renderer in m_renderers)
            {
                if (renderer.gameObject.name.Contains("Body") || 
                    renderer.gameObject.name.Contains("Roof") ||
                    renderer.gameObject.name.Contains("Hood") ||
                    renderer.gameObject.name.Contains("Trunk"))
                {
                    renderer.material.color = m_bodyColor;
                }
            }
        }

        public void SetLocked(bool locked)
        {
            m_isLocked = locked;
        }

        public void ApplyDamage(float amount)
        {
            m_damageLevel = Mathf.Clamp01(m_damageLevel + amount);
            
            UpdateVisualDamage();
        }

        private void UpdateVisualDamage()
        {
            if (m_damageLevel > 0.5f && m_renderers != null)
            {
                foreach (var renderer in m_renderers)
                {
                    renderer.material.color = Color.Lerp(renderer.material.color, Color.gray, 0.3f);
                }
            }
        }

        public string GetCarId() => m_carId;
        public ParkedCarType GetCarType() => m_carType;
        public bool IsLocked() => m_isLocked;
        public float GetDamageLevel() => m_damageLevel;
        public ParkedCarSpawnPoint GetSpawnPoint() => m_spawnPoint;

        public void OnCarEnter()
        {
            Debug.Log($"[ParkedCar] Entered {name}");
        }
    }
}
