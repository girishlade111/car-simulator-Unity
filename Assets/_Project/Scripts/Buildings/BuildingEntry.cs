using UnityEngine;

public class BuildingEntry : MonoBehaviour
{
    [Header("Entry Settings")]
    [SerializeField] private string m_buildingName;
    [SerializeField] private string m_buildingId;
    [SerializeField] private bool m_isLocked = false;

    [Header("Interior Reference")]
    [SerializeField] private Transform m_interiorSpawnPoint;
    [SerializeField] private GameObject m_interiorPrefab;

    [Header("Interaction")]
    [SerializeField] private float m_interactionRadius = 3f;
    [SerializeField] private KeyCode m_interactionKey = KeyCode.E;

    private bool m_isPlayerInRange;

    public string BuildingName => m_buildingName;
    public string BuildingId => m_buildingId;
    public bool IsLocked => m_isLocked;
    public Transform InteriorSpawnPoint => m_interiorSpawnPoint;

    private void Update()
    {
        if (m_isPlayerInRange && Input.GetKeyDown(m_interactionKey))
        {
            TryEnterBuilding();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isPlayerInRange = true;
            ShowInteractionPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isPlayerInRange = false;
            ShowInteractionPrompt(false);
        }
    }

    private void TryEnterBuilding()
    {
        if (m_isLocked)
        {
            Debug.Log($"[BuildingEntry] {m_buildingName} is locked");
            return;
        }

        if (m_interiorPrefab != null)
        {
            EnterInterior();
        }
        else
        {
            Debug.Log($"[BuildingEntry] Interior not implemented for {m_buildingName}");
        }
    }

    private void EnterInterior()
    {
        Debug.Log($"[BuildingEntry] Entering {m_buildingName}");
        // TODO: Implement interior loading
    }

    private void ShowInteractionPrompt(bool show)
    {
        // TODO: Connect to UI system
        if (show && !m_isLocked)
        {
            Debug.Log($"[BuildingEntry] Press E to enter {m_buildingName}");
        }
        else if (show && m_isLocked)
        {
            Debug.Log($"[BuildingEntry] {m_buildingName} is locked");
        }
    }

    public void SetLocked(bool locked)
    {
        m_isLocked = locked;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = m_isLocked ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, m_interactionRadius);

        if (m_interiorSpawnPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, m_interiorSpawnPoint.position);
        }
    }
}
