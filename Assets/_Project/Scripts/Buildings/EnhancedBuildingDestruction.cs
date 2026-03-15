using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class EnhancedBuildingDestruction : MonoBehaviour
    {
        [Header("Destruction Levels")]
        [SerializeField] private DestructionLevel m_currentLevel = DestructionLevel.Intact;
        [SerializeField] private float[] m_levelThresholds = { 100f, 75f, 50f, 25f, 0f };

        [Header("Health")]
        [SerializeField] private float m_maxHealth = 100f;
        [SerializeField] private float m_currentHealth;

        [Header("Level 1 - Minor Damage")]
        [SerializeField] private bool m_enableLevel1 = true;
        [SerializeField] private Color m_level1Tint = new Color(1f, 0.95f, 0.9f);
        [SerializeField] private GameObject m_cracksPrefab;
        [SerializeField] private float m_cracksIntensity = 0.3f;

        [Header("Level 2 - Moderate Damage")]
        [SerializeField] private bool m_enableLevel2 = true;
        [SerializeField] private Color m_level2Tint = new Color(1f, 0.85f, 0.8f);
        [SerializeField] private GameObject m_debrisLevel2;
        [SerializeField] private int m_debrisCountLevel2 = 5;

        [Header("Level 3 - Heavy Damage")]
        [SerializeField] private bool m_enableLevel3 = true;
        [SerializeField] private Color m_level3Tint = new Color(1f, 0.7f, 0.6f);
        [SerializeField] private GameObject m_debrisLevel3;
        [SerializeField] private int m_debrisCountLevel3 = 10;
        [SerializeField] private Transform[] m_collapsePoints;

        [Header("Level 4 - Critical")]
        [SerializeField] private bool m_enableLevel4 = true;
        [SerializeField] private Color m_level4Tint = new Color(0.8f, 0.6f, 0.5f);
        [SerializeField] private bool m_partialCollapse = true;
        [SerializeField] private float m_collapseForce = 500f;

        [Header("Level 5 - Destroyed")]
        [SerializeField] private bool m_enableLevel5 = true;
        [SerializeField] private GameObject m_finalDebris;
        [SerializeField] private int m_finalDebrisCount = 20;
        [SerializeField] private float m_explosionForce = 1000f;

        [Header("Audio")]
        [SerializeField] private AudioClip[] m_damageSounds;
        [SerializeField] private AudioClip[] m_collapseSounds;
        [SerializeField] private AudioClip[] m_destructionSounds;

        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem m_dustParticles;
        [SerializeField] private ParticleSystem m_sparksParticles;
        [SerializeField] private ParticleSystem m_smokeParticles;

        private Renderer[] m_renderers;
        private List<GameObject> m_spawnedEffects = new List<GameObject>();
        private List<Rigidbody> m_collapsePieces = new List<Rigidbody>();
        private bool m_hasPlayedDestruction;

        public enum DestructionLevel
        {
            Intact,
            MinorDamage,
            ModerateDamage,
            HeavyDamage,
            Critical,
            Destroyed
        }

        private void Start()
        {
            m_currentHealth = m_maxHealth;
            GetRenderers();
        }

        private void GetRenderers()
        {
            m_renderers = GetComponentsInChildren<Renderer>();
        }

        public void TakeDamage(float damage)
        {
            m_currentHealth = Mathf.Max(0, m_currentHealth - damage);

            UpdateDestructionLevel();
            PlayDamageEffect();
            UpdateVisuals();

            if (m_currentHealth <= 0)
            {
                DestroyBuilding();
            }
        }

        private void UpdateDestructionLevel()
        {
            float healthPercent = (m_currentHealth / m_maxHealth) * 100f;

            DestructionLevel newLevel = m_currentLevel;

            if (healthPercent >= m_levelThresholds[0])
                newLevel = DestructionLevel.Intact;
            else if (healthPercent >= m_levelThresholds[1] && m_enableLevel1)
                newLevel = DestructionLevel.MinorDamage;
            else if (healthPercent >= m_levelThresholds[2] && m_enableLevel2)
                newLevel = DestructionLevel.ModerateDamage;
            else if (healthPercent >= m_levelThresholds[3] && m_enableLevel3)
                newLevel = DestructionLevel.HeavyDamage;
            else if (healthPercent >= m_levelThresholds[4] && m_enableLevel4)
                newLevel = DestructionLevel.Critical;
            else if (m_enableLevel5)
                newLevel = DestructionLevel.Destroyed;

            if (newLevel != m_currentLevel)
            {
                OnLevelChange(newLevel);
            }
        }

        private void OnLevelChange(DestructionLevel newLevel)
        {
            switch (newLevel)
            {
                case DestructionLevel.MinorDamage:
                    ApplyMinorDamage();
                    break;
                case DestructionLevel.ModerateDamage:
                    ApplyModerateDamage();
                    break;
                case DestructionLevel.HeavyDamage:
                    ApplyHeavyDamage();
                    break;
                case DestructionLevel.Critical:
                    ApplyCriticalDamage();
                    break;
                case DestructionLevel.Destroyed:
                    DestroyBuilding();
                    break;
            }
        }

        private void ApplyMinorDamage()
        {
            if (m_dustParticles != null)
            {
                m_dustParticles.Play();
            }

            if (m_cracksPrefab != null)
            {
                SpawnEffect(m_cracksPrefab, 3);
            }

            PlaySound(m_damageSounds);
        }

        private void ApplyModerateDamage()
        {
            if (m_debrisLevel2 != null)
            {
                SpawnEffect(m_debrisLevel2, m_debrisCountLevel2);
            }

            if (m_sparksParticles != null)
            {
                m_sparksParticles.Play();
            }

            PlaySound(m_damageSounds);
        }

        private void ApplyHeavyDamage()
        {
            if (m_debrisLevel3 != null)
            {
                SpawnEffect(m_debrisLevel3, m_debrisCountLevel3);
            }

            if (m_smokeParticles != null)
            {
                m_smokeParticles.Play();
            }

            if (m_partialCollapse && m_collapsePoints != null)
            {
                TriggerPartialCollapse();
            }

            PlaySound(m_collapseSounds);
        }

        private void ApplyCriticalDamage()
        {
            if (m_collapsePoints != null)
            {
                foreach (var point in m_collapsePoints)
                {
                    if (point != null)
                    {
                        Rigidbody rb = point.gameObject.AddComponent<Rigidbody>();
                        rb.mass = 200f;
                        rb.AddExplosionForce(m_collapseForce, point.position, 5f);
                        m_collapsePieces.Add(rb);
                    }
                }
            }

            PlaySound(m_collapseSounds);
        }

        private void TriggerPartialCollapse()
        {
            if (m_collapsePoints == null) return;

            int collapseCount = Mathf.Min(3, m_collapsePoints.Length);

            for (int i = 0; i < collapseCount; i++)
            {
                if (m_collapsePoints[i] != null)
                {
                    Rigidbody rb = m_collapsePoints[i].gameObject.AddComponent<Rigidbody>();
                    rb.mass = Random.Range(100f, 300f);
                    rb.isKinematic = false;

                    Vector3 force = (m_collapsePoints[i].position - transform.position).normalized + Vector3.down;
                    rb.AddForce(force * m_collapseForce * 0.5f, ForceMode.Impulse);

                    m_collapsePieces.Add(rb);
                }
            }
        }

        private void DestroyBuilding()
        {
            if (m_hasPlayedDestruction) return;
            m_hasPlayedDestruction = true;

            if (m_finalDebris != null)
            {
                SpawnEffect(m_finalDebris, m_finalDebrisCount);
            }

            SpawnPrimitiveDebris();

            ApplyExplosionForce();

            DisableColliders();

            PlaySound(m_destructionSounds);

            if (m_smokeParticles != null)
            {
                m_smokeParticles.Play();
            }

            Destroy(gameObject, 10f);
        }

        private void SpawnPrimitiveDebris()
        {
            for (int i = 0; i < m_finalDebrisCount; i++)
            {
                GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
                debris.transform.SetParent(transform.parent);
                debris.transform.position = transform.position + Random.insideUnitSphere * 2f;
                debris.transform.rotation = Random.rotation;
                debris.transform.localScale = Random.Range(0.2f, 1f) * Vector3.one;

                Renderer renderer = debris.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(0.4f, 0.35f, 0.3f);
                }

                Rigidbody rb = debris.AddComponent<Rigidbody>();
                rb.mass = Random.Range(5f, 30f);
                rb.AddExplosionForce(m_explosionForce * 0.5f, transform.position, 10f);

                Destroy(debris, Random.Range(5f, 15f));
            }
        }

        private void ApplyExplosionForce()
        {
            Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
            foreach (var body in bodies)
            {
                body.AddExplosionForce(m_explosionForce, transform.position, 20f);
            }
        }

        private void DisableColliders()
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
        }

        private void SpawnEffect(GameObject prefab, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * 2f;
                GameObject effect = Instantiate(prefab, pos, Random.rotation);
                m_spawnedEffects.Add(effect);

                Destroy(effect, 5f);
            }
        }

        private void UpdateVisuals()
        {
            if (m_renderers == null) return;

            Color tint = GetCurrentTint();

            foreach (var renderer in m_renderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = tint;
                }
            }
        }

        private Color GetCurrentTint()
        {
            switch (m_currentLevel)
            {
                case DestructionLevel.Intact:
                    return Color.white;
                case DestructionLevel.MinorDamage:
                    return m_level1Tint;
                case DestructionLevel.ModerateDamage:
                    return m_level2Tint;
                case DestructionLevel.HeavyDamage:
                    return m_level3Tint;
                case DestructionLevel.Critical:
                    return m_level4Tint;
                default:
                    return Color.gray;
            }
        }

        private void PlayDamageEffect()
        {
            if (m_damageSounds != null && m_damageSounds.Length > 0)
            {
                AudioSource audio = gameObject.AddComponent<AudioSource>();
                audio.PlayOneShot(m_damageSounds[Random.Range(0, m_damageSounds.Length)]);
                Destroy(audio, 2f);
            }
        }

        private void PlaySound(AudioClip[] sounds)
        {
            if (sounds == null || sounds.Length == 0) return;

            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
            Destroy(audio, 3f);
        }

        public void Repair(float amount)
        {
            m_currentHealth = Mathf.Min(m_maxHealth, m_currentHealth + amount);
            UpdateDestructionLevel();
            UpdateVisuals();
        }

        public void SetHealth(float health)
        {
            m_maxHealth = health;
            m_currentHealth = health;
        }

        public float GetHealthPercentage() => m_currentHealth / m_maxHealth;
        public DestructionLevel GetDestructionLevel() => m_currentLevel;
        public bool IsDestroyed() => m_currentLevel == DestructionLevel.Destroyed;
    }

    public class DestructibleComponent : MonoBehaviour
    {
        [Header("Component Health")]
        [SerializeField] private float m_health = 20f;
        [SerializeField] private bool m_detachOnDestroy = true;

        [Header("Effects")]
        [SerializeField] private GameObject m_destroyEffect;
        [SerializeField] private AudioClip m_breakSound;

        private float m_currentHealth;
        private bool m_isDestroyed;

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
                DestroyComponent();
            }
        }

        private void DestroyComponent()
        {
            m_isDestroyed = true;

            if (m_destroyEffect != null)
            {
                Instantiate(m_destroyEffect, transform.position, transform.rotation);
            }

            if (m_breakSound != null)
            {
                AudioSource.PlayClipAtPoint(m_breakSound, transform.position);
            }

            if (m_detachOnDestroy)
            {
                DetachFromParent();
            }

            Destroy(gameObject);
        }

        private void DetachFromParent()
        {
            transform.parent = null;

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 50f;
            rb.AddExplosionForce(200f, transform.position, 3f);

            Destroy(rb, 5f);
            Destroy(gameObject, 10f);
        }
    }
}
