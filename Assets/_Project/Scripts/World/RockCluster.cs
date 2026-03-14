using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class RockCluster : MonoBehaviour
    {
        [Header("Rock Settings")]
        [SerializeField] private GameObject m_rockPrefab;
        [SerializeField] private int m_rockCount = 5;
        [SerializeField] private Vector2 m_clusterRadius = new Vector2(2f, 5f);
        [SerializeField] private Vector2 m_rockScaleRange = new Vector2(0.5f, 1.5f);

        [Header("Individual Rock")]
        [SerializeField] private Vector2 m_individualScaleX = new Vector2(0.8f, 2f);
        [SerializeField] private Vector2 m_individualScaleY = new Vector2(0.4f, 1f);
        [SerializeField] private Vector2 m_individualScaleZ = new Vector2(0.8f, 2f);
        [SerializeField] private Vector2 m_rotationVariation = new Vector2(-15f, 15f);

        [Header("Materials")]
        [SerializeField] private Material m_rockMaterial;
        [SerializeField] private Color m_rockColor = Color.gray;

        [Header("Placement")]
        [SerializeField] private bool m_autoSpawnOnStart = true;
        [SerializeField] private float m_groundOffset = 0.2f;
        [SerializeField] private bool m_buryRocks = true;
        [SerializeField] private Vector2 m_buryDepth = new Vector2(0f, 0.3f);

        private List<GameObject> m_spawnedRocks = new List<GameObject>();

        private void Start()
        {
            if (m_autoSpawnOnStart)
            {
                SpawnCluster();
            }
        }

        public void SpawnCluster()
        {
            ClearCluster();

            float clusterRadius = Random.Range(m_clusterRadius.x, m_clusterRadius.y);

            for (int i = 0; i < m_rockCount; i++)
            {
                Vector2 circle = Random.insideUnitCircle * clusterRadius;
                Vector3 offset = new Vector3(circle.x, 0, circle.y);

                GameObject rock = SpawnRock(transform.position + offset);
                if (rock != null)
                {
                    m_spawnedRocks.Add(rock);
                }
            }
        }

        private GameObject SpawnRock(Vector3 position)
        {
            GameObject rock;

            if (m_rockPrefab != null)
            {
                rock = Instantiate(m_rockPrefab, position, Random.rotation, transform);
            }
            else
            {
                rock = CreatePlaceholderRock(position);
            }

            float scaleX = Random.Range(m_individualScaleX.x, m_individualScaleX.y);
            float scaleY = Random.Range(m_individualScaleY.x, m_individualScaleY.y);
            float scaleZ = Random.Range(m_individualScaleZ.x, m_individualScaleZ.y);
            rock.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

            rock.transform.Rotate(
                Random.Range(m_rotationVariation.x, m_rotationVariation.y),
                Random.Range(0, 360),
                Random.Range(m_rotationVariation.x, m_rotationVariation.y)
            );

            if (m_buryRocks)
            {
                float buryDepth = Random.Range(m_buryDepth.x, m_buryDepth.y);
                rock.transform.position -= Vector3.up * buryDepth * scaleY;
            }

            ApplyMaterial(rock);

            return rock;
        }

        private GameObject CreatePlaceholderRock(Vector3 position)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.transform.position = position + Vector3.up * m_groundOffset;
            rock.transform.SetParent(transform);
            rock.name = "Rock";

            return rock;
        }

        private void ApplyMaterial(GameObject rock)
        {
            if (m_rockMaterial != null)
            {
                Renderer renderer = rock.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = m_rockMaterial;
                }
            }
            else
            {
                Renderer[] renderers = rock.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    Material mat = new Material(Shader.Find("Standard"));
                    float variation = Random.Range(-0.1f, 0.1f);
                    mat.color = new Color(
                        Mathf.Clamp01(m_rockColor.r + variation),
                        Mathf.Clamp01(m_rockColor.g + variation),
                        Mathf.Clamp01(m_rockColor.b + variation)
                    );
                    renderer.material = mat;
                }
            }
        }

        public void ClearCluster()
        {
            foreach (var rock in m_spawnedRocks)
            {
                if (rock != null)
                {
                    Destroy(rock);
                }
            }
            m_spawnedRocks.Clear();
        }

        public int GetRockCount() => m_spawnedRocks.Count;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            float radius = (m_clusterRadius.x + m_clusterRadius.y) / 2f;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
