using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WheelCollider[] m_frontWheels;
    [SerializeField] private WheelCollider[] m_rearWheels;
    [SerializeField] private Transform m_centerOfMass;

    [Header("Tuning")]
    [SerializeField] private float m_maxSteerAngle = 35f;
    [SerializeField] private float m_maxEngineForce = 1500f;
    [SerializeField] private float m_maxBrakeForce = 3000f;
    [SerializeField] private float m_downforce = 10f;

    [Header("Stability")]
    [SerializeField] private float m_antiRollBarForce = 500f;
    [SerializeField] private float m_brakeBalance = 0.6f;

    [Header("Respawn")]
    [SerializeField] private float m_respawnHeight = -10f;
    [SerializeField] private float m_resetDelay = 2f;

    private Rigidbody m_rb;
    private InputHandler m_input;
    private float m_respawnTimer;

    public float CurrentSpeed { get; private set; }
    public float MaxSpeed => 150f;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_input = InputHandler.Instance;

        if (m_centerOfMass != null)
        {
            m_rb.centerOfMass = m_centerOfMass.localPosition;
        }

        m_rb.mass = 1500f;
    }

    private void Update()
    {
        if (m_input.PauseInput)
        {
            GameManager.Instance.TogglePause();
        }

        if (transform.position.y < m_respawnHeight)
        {
            m_respawnTimer += Time.deltaTime;
            if (m_respawnTimer >= m_resetDelay)
            {
                Respawn();
            }
        }
        else
        {
            m_respawnTimer = 0f;
        }

        if (m_input.ResetInput)
        {
            Respawn();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsPaused) return;

        ApplySteering();
        ApplyEngineForce();
        ApplyBraking();
        ApplyDownforce();
        Stabilize();

        CurrentSpeed = m_rb.velocity.magnitude * 3.6f;
    }

    private void ApplySteering()
    {
        float steer = m_input.SteeringInput * m_maxSteerAngle;

        foreach (var wheel in m_frontWheels)
        {
            wheel.steerAngle = steer;
        }
    }

    private void ApplyEngineForce()
    {
        float force = m_input.ThrottleInput * m_maxEngineForce;

        foreach (var wheel in m_rearWheels)
        {
            wheel.motorTorque = force;
        }
    }

    private void ApplyBraking()
    {
        float brake = 0f;

        if (m_input.BrakeInput)
        {
            brake = m_maxBrakeForce;
        }
        else if (m_input.ThrottleInput < 0f && CurrentSpeed > 1f)
        {
            brake = m_maxBrakeForce * 0.5f;
        }

        float frontBrake = brake * m_brakeBalance;
        float rearBrake = brake * (1f - m_brakeBalance);

        foreach (var wheel in m_frontWheels)
        {
            wheel.brakeTorque = frontBrake;
        }

        foreach (var wheel in m_rearWheels)
        {
            wheel.brakeTorque = rearBrake;
        }
    }

    private void ApplyDownforce()
    {
        m_rb.AddForce(-transform.up * m_downforce * CurrentSpeed * 0.01f, ForceMode.Acceleration);
    }

    private void Stabilize()
    {
        if (m_frontWheels.Length < 2 || m_rearWheels.Length < 2) return;

        WheelHit hit;
        float travelL = m_frontWheels[0].GetGroundHit(out hit) ? hit.force : 0f;
        float travelR = m_frontWheels[1].GetGroundHit(out hit) ? hit.force : 0f;

        float antiRoll = (travelL - travelR) * m_antiRollBarForce;
        m_rb.AddForceAtPosition(transform.right * antiRoll, m_frontWheels[0].transform.position);
        m_rb.AddForceAtPosition(transform.right * -antiRoll, m_frontWheels[1].transform.position);
    }

    public void Respawn()
    {
        m_rb.velocity = Vector3.zero;
        m_rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        Vector3 pos = transform.position;
        pos.y = Mathf.Max(pos.y + 2f, 2f);
        transform.position = pos;

        m_respawnTimer = 0f;
        Debug.Log("[Vehicle] Respawned");
    }
}
