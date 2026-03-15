using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class BuildingDestruction : MonoBehaviour
    {
        [Header("Destruction Settings")]
        [SerializeField] private bool m_enableDestruction = true;
        [SerializeField] private float m_health = 100f;
        [SerializeField] private float m_currentHealth;

        [Header("Damage")]
        [SerializeField] private float m_vehicleImpactMultiplier = 1f;
        [SerializeField] private float m_explosionMultiplier = 2f;
        [SerializeField] private float m_fallDamageThreshold = 10f;

        [Header("Debris")]
        [SerializeField] private GameObject m_debrisPrefab;
        [SerializeField] private int m_debrisCount = 10;
        [SerializeField] private float m_debrisSpread = 5f;

        [Header("Collapse")]
        [SerializeField] private bool m_canCollapse = true;
        [SerializeField] private float m_collapseThreshold = 0.3f;
        [SerializeField] private Transform[] m_destructibleParts;

        [Header("Audio")]
        [SerializeField] private AudioClip[] m_damageSounds;
        [SerializeField] private AudioClip[] m_collapseSounds;

        private bool m_isDestroyed;
        private bool m_isCollapsing;
        private List<Rigidbody> m_debrisPieces = new List<Rigidbody>();

        public float Health => m_currentHealth;
        public float MaxHealth => m_health;
        public bool IsDestroyed => m_isDestroyed;

        private void Start()
        {
            m_currentHealth = m_health;
        }

        public void TakeDamage(float damage)
        {
            if (m_isDestroyed) return;

            m_currentHealth -= damage;

            if (m_currentHealth <= 0)
            {
                DestroyBuilding();
            }
            else if (m_canCollapse && m_currentHealth < m_health * m_collapseThreshold)
            {
                StartCollapse();
            }
        }

        public void TakeVehicleImpact(float impactForce)
        {
            float damage = impactForce * m_vehicleImpactMultiplier;
            TakeDamage(damage);
        }

        public void TakeExplosionDamage(Vector3 explosionPoint, float explosionForce)
        {
            float distance = Vector3.Distance(explosionPoint, transform.position);
            float damage = explosionForce * m_explosionMultiplier / (1f + distance * 0.1f);
            TakeDamage(damage);
        }

        private void DestroyBuilding()
        {
            m_isDestroyed = true;
            m_currentHealth = 0;

            SpawnDebris();
            DisableBuilding();
            PlayCollapseSound();

            Debug.Log($"[BuildingDestruction] Building destroyed: {gameObject.name}");
        }

        private void SpawnDebris()
        {
            if (m_debrisPrefab != null)
            {
                for (int i = 0; i < m_debrisCount; i++)
                {
                    Vector3 spawnPos = transform.position + Random.insideUnitSphere * m_debrisSpread;
                    GameObject debris = Instantiate(m_debrisPrefab, spawnPos, Random.rotation);
                    AddDebrisPhysics(debris);
                }
            }

            SpawnPrimitiveDebris();
        }

        private void SpawnPrimitiveDebris()
        {
            for (int i = 0; i < m_debrisCount / 2; i++)
            {
                GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
                debris.transform.SetParent(transform.parent);
                debris.transform.position = transform.position + Random.insideUnitSphere * m_debrisSpread;
                debris.transform.rotation = Random.rotation;
                debris.transform.localScale = Random.Range(0.2f, 1f) * Vector3.one;

                Renderer renderer = debris.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(0.5f, 0.45f, 0.4f);
                }

                AddDebrisPhysics(debris);
            }
        }

        private void AddDebrisPhysics(GameObject obj)
        {
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.mass = Random.Range(10f, 50f);
            rb.AddExplosionForce(500f, transform.position, m_debrisSpread * 2f);

            m_debrisPieces.Add(rb);

            Destroy(obj, Random.Range(5f, 15f));
        }

        private void DisableBuilding()
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer != null)
                {
                    renderer.material.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                }
            }
        }

        private void StartCollapse()
        {
            if (m_isCollapsing) return;
            m_isCollapsing = true;

            if (m_destructibleParts != null)
            {
                foreach (var part in m_destructibleParts)
                {
                    if (part != null)
                    {
                        Rigidbody rb = part.gameObject.AddComponent<Rigidbody>();
                        rb.mass = 500f;
                        rb.isKinematic = false;

                        Vector3 fallDirection = (part.position - transform.position).normalized + Vector3.down;
                        rb.AddForce(fallDirection * 200f, ForceMode.Impulse);
                    }
                }
            }

            PlayCollapseSound();
        }

        private void PlayCollapseSound()
        {
            if (m_collapseSounds != null && m_collapseSounds.Length > 0)
            {
                AudioSource audio = gameObject.AddComponent<AudioSource>();
                audio.PlayOneShot(m_collapseSounds[Random.Range(0, m_collapseSounds.Length)]);
            }
        }

        public void Repair(float amount)
        {
            m_currentHealth = Mathf.Min(m_currentHealth + amount, m_health);

            if (m_currentHealth >= m_health * 0.5f && m_isCollapsing)
            {
                StopCollapse();
            }
        }

        private void StopCollapse()
        {
            m_isCollapsing = false;

            if (m_destructibleParts != null)
            {
                foreach (var part in m_destructibleParts)
                {
                    if (part != null)
                    {
                        Rigidbody rb = part.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            Destroy(rb);
                        }

                        part.localPosition = Vector3.zero;
                        part.localRotation = Quaternion.identity;
                    }
                }
            }
        }

        public float GetHealthPercentage() => m_currentHealth / m_health;

        public bool IsDamaged() => m_currentHealth < m_health;

        private void OnCollisionEnter(Collision collision)
        {
            if (!m_enableDestruction) return;

            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Vehicle"))
            {
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float impactForce = rb.velocity.magnitude * rb.mass;
                    TakeVehicleImpact(impactForce);
                }
            }
        }
    }

    public class DestructibleWall : MonoBehaviour
    {
        [Header("Wall Settings")]
        [SerializeField] private float m_health = 50f;
        [SerializeField] private int m_widthSections = 4;
        [SerializeField] private int m_heightSections = 3;

        [Header("Effects")]
        [SerializeField] private GameObject m_destroyEffect;
        [SerializeField] private AudioClip m_breakSound;

        private float m_currentHealth;
        private Renderer m_renderer;
        private bool m_isDestroyed;

        private void Start()
        {
            m_currentHealth = m_health;
            m_renderer = GetComponent<Renderer>();
        }

        public void TakeDamage(float damage)
        {
            if (m_isDestroyed) return;

            m_currentHealth -= damage;

            UpdateVisual();

            if (m_currentHealth <= 0)
            {
                DestroyWall();
            }
        }

        private void UpdateVisual()
        {
            if (m_renderer == null) return;

            float healthPercent = m_currentHealth / m_health;
            Color damagedColor = Color.Lerp(Color.red, Color.white, healthPercent);
            m_renderer.material.color = damagedColor;
        }

        private void DestroyWall()
        {
            m_isDestroyed = true;

            if (m_destroyEffect != null)
            {
                Instantiate(m_destroyEffect, transform.position, Quaternion.identity);
            }

            if (m_breakSound != null)
            {
                AudioSource.PlayClipAtPoint(m_breakSound, transform.position);
            }

            SpawnDebris();

            Destroy(gameObject);
        }

        private void SpawnDebris()
        {
            for (int i = 0; i < 8; i++)
            {
                GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
                debris.transform.position = transform.position + Random.insideUnitSphere;
                debris.transform.localScale = Random.Range(0.1f, 0.3f) * Vector3.one;

                Rigidbody rb = debris.AddComponent<Rigidbody>();
                rb.AddExplosionForce(100f, transform.position, 3f);

                Destroy(debris, 3f);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            float impact = collision.relativeVelocity.magnitude;
            if (impact > 5f)
            {
                TakeDamage(impact * 5f);
            }
        }
    }
}
