using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class BuildingFireSystem : MonoBehaviour
    {
        public static BuildingFireSystem Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enableFire = true;
        [SerializeField] private float m_fireSpreadInterval = 5f;
        [SerializeField] private float m_fireDamagePerSecond = 10f;
        [SerializeField] private float m_spreadRadius = 8f;

        [Header("Fire Settings")]
        [SerializeField] private int m_maxFires = 10;
        [SerializeField] private float m_igniteTemperature = 100f;
        [SerializeField] private float m_fireHealthDamage = 5f;

        [Header("Effects")]
        [SerializeField] private ParticleSystem m_fireParticlePrefab;
        [SerializeField] private ParticleSystem m_smokeParticlePrefab;
        [SerializeField] private Light m_fireLightPrefab;
        [SerializeField] private Color m_fireColor = new Color(1f, 0.5f, 0.1f);

        [Header("Audio")]
        [SerializeField] private AudioClip[] m_fireSounds;

        private List<FireInstance> m_activeFires = new List<FireInstance>();
        private float m_spreadTimer;

        [System.Serializable]
        public class FireInstance
        {
            public Vector3 position;
            public Transform target;
            public ParticleSystem fireParticles;
            public ParticleSystem smokeParticles;
            public Light fireLight;
            public float intensity;
            public float age;
            public bool isSpreading;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (!m_enableFire) return;

            UpdateFires();
            HandleSpread();
        }

        private void UpdateFires()
        {
            for (int i = m_activeFires.Count - 1; i >= 0; i--)
            {
                FireInstance fire = m_activeFires[i];
                fire.age += Time.deltaTime;

                UpdateFireEffects(fire);
                ApplyFireDamage(fire);

                if (fire.age > 60f)
                {
                    ExtinguishFire(fire);
                }
            }
        }

        private void UpdateFireEffects(FireInstance fire)
        {
            if (fire.fireParticles != null)
            {
                var main = fire.fireParticles.main;
                main.startColor = new Color(m_fireColor.r, m_fireColor.g, m_fireColor.b, fire.intensity);
                main.startSpeed = 2f + fire.intensity * 3f;
            }

            if (fire.fireLight != null)
            {
                fire.fireLight.intensity = fire.intensity * 2f;
                fire.fireLight.color = m_fireColor;
                fire.fireLight.range = 5f + fire.intensity * 5f;

                fire.fireLight.transform.position = fire.position + Vector3.up * 2f;
            }
        }

        private void ApplyFireDamage(FireInstance fire)
        {
            if (fire.target == null) return;

            var destruction = fire.target.GetComponent<Buildings.EnhancedBuildingDestruction>();
            if (destruction != null)
            {
                destruction.TakeDamage(m_fireDamagePerSecond * Time.deltaTime * fire.intensity);
            }

            var nearbyObjects = Physics.OverlapSphere(fire.position, m_spreadRadius);
            foreach (var obj in nearbyObjects)
            {
                if (obj.CompareTag("Building") || obj.CompareTag("Prop"))
                {
                    var nearbyDestruction = obj.GetComponent<Buildings.EnhancedBuildingDestruction>();
                    if (nearbyDestruction != null && Random.value < 0.01f)
                    {
                        nearbyDestruction.TakeDamage(m_fireHealthDamage * Time.deltaTime);
                    }
                }
            }
        }

        private void HandleSpread()
        {
            m_spreadTimer += Time.deltaTime;

            if (m_spreadTimer >= m_fireSpreadInterval)
            {
                m_spreadTimer = 0f;

                if (m_activeFires.Count < m_maxFires)
                {
                    SpreadFire();
                }
            }
        }

        private void SpreadFire()
        {
            if (m_activeFires.Count == 0) return;

            FireInstance sourceFire = m_activeFires[Random.Range(0, m_activeFires.Count)];

            Collider[] nearby = Physics.OverlapSphere(sourceFire.position, m_spreadRadius);
            List<Transform> validTargets = new List<Transform>();

            foreach (var collider in nearby)
            {
                if (collider.CompareTag("Building") || collider.CompareTag("Prop"))
                {
                    if (!IsOnFire(collider.transform))
                    {
                        validTargets.Add(collider.transform);
                    }
                }
            }

            if (validTargets.Count > 0 && Random.value < 0.3f)
            {
                Transform target = validTargets[Random.Range(0, validTargets.Count)];
                Ignite(target.position + Vector3.up * 2f, target);
            }
        }

        private bool IsOnFire(Transform target)
        {
            foreach (var fire in m_activeFires)
            {
                if (fire.target == target) return true;
            }
            return false;
        }

        public void Ignite(Vector3 position, Transform target = null)
        {
            if (m_activeFires.Count >= m_maxFires) return;

            FireInstance fire = new FireInstance
            {
                position = position,
                target = target,
                intensity = 1f,
                age = 0,
                isSpreading = true
            };

            fire.fireParticles = CreateFireParticles(position);
            fire.smokeParticles = CreateSmokeParticles(position);
            fire.fireLight = CreateFireLight(position);

            m_activeFires.Add(fire);

            Debug.Log($"[BuildingFire] Ignited at {position}");
        }

        private ParticleSystem CreateFireParticles(Vector3 position)
        {
            if (m_fireParticlePrefab != null)
            {
                return Instantiate(m_fireParticlePrefab, position, Quaternion.identity);
            }

            GameObject particles = new GameObject("FireParticles");
            particles.transform.position = position;

            ParticleSystem ps = particles.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 1f;
            main.startSpeed = 2f;
            main.startSize = 1f;
            main.maxParticles = 50;

            var emission = ps.emission;
            emission.rateOverTime = 20;

            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            renderer.material.color = m_fireColor;

            return ps;
        }

        private ParticleSystem CreateSmokeParticles(Vector3 position)
        {
            if (m_smokeParticlePrefab != null)
            {
                return Instantiate(m_smokeParticlePrefab, position + Vector3.up * 3f, Quaternion.identity);
            }

            GameObject smoke = new GameObject("SmokeParticles");
            smoke.transform.position = position + Vector3.up * 3f;

            ParticleSystem ps = smoke.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 3f;
            main.startSpeed = 1f;
            main.startSize = 2f;

            var emission = ps.emission;
            emission.rateOverTime = 10;

            return ps;
        }

        private Light CreateFireLight(Vector3 position)
        {
            GameObject lightObj = new GameObject("FireLight");
            lightObj.transform.position = position + Vector3.up * 2f;

            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = m_fireColor;
            light.intensity = 2f;
            light.range = 10f;

            return light;
        }

        public void ExtinguishFire(FireInstance fire)
        {
            if (fire.fireParticles != null)
            {
                Destroy(fire.fireParticles.gameObject);
            }

            if (fire.smokeParticles != null)
            {
                Destroy(fire.smokeParticles.gameObject);
            }

            if (fire.fireLight != null)
            {
                Destroy(fire.fireLight.gameObject);
            }

            m_activeFires.Remove(fire);
        }

        public void ExtinguishAll()
        {
            for (int i = m_activeFires.Count - 1; i >= 0; i--)
            {
                ExtinguishFire(m_activeFires[i]);
            }
        }

        public void ExtinguishAt(Vector3 position, float radius)
        {
            for (int i = m_activeFires.Count - 1; i >= 0; i--)
            {
                if (Vector3.Distance(m_activeFires[i].position, position) < radius)
                {
                    ExtinguishFire(m_activeFires[i]);
                }
            }
        }

        public void IgniteBuilding(Transform building)
        {
            Vector3 ignitePoint = building.position + Random.insideUnitSphere * 3f;
            ignitePoint.y = building.position.y + 2f;
            Ignite(ignitePoint, building);
        }

        public int GetActiveFireCount() => m_activeFires.Count;
        public bool IsOnFire() => m_activeFires.Count > 0;
    }

    public class FireHydrant : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float m_waterRange = 10f;
        [SerializeField] private float m_waterForce = 5f;
        [SerializeField] private KeyCode m_useKey = KeyCode.F;

        [Header("Effects")]
        [SerializeField] private ParticleSystem m_waterParticles;
        [SerializeField] private AudioSource m_audioSource;

        private bool m_isActive;

        private void Start()
        {
            if (m_waterParticles != null)
            {
                m_waterParticles.Stop();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(m_useKey))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            m_isActive = !m_isActive;

            if (m_isActive)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        private void Activate()
        {
            if (m_waterParticles != null)
            {
                m_waterParticles.Play();
            }

            if (m_audioSource != null)
            {
                m_audioSource.Play();
            }

            ExtinguishNearbyFire();

            Invoke(nameof(Deactivate), 10f);
        }

        private void Deactivate()
        {
            m_isActive = false;

            if (m_waterParticles != null)
            {
                m_waterParticles.Stop();
            }
        }

        private void ExtinguishNearbyFire()
        {
            if (BuildingFireSystem.Instance != null)
            {
                BuildingFireSystem.Instance.ExtinguishAt(transform.position, m_waterRange);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, m_waterRange);
        }
    }
}
