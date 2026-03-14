using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private float m_shakeDuration = 0.3f;
    [SerializeField] private float m_shakeMagnitude = 0.2f;

    private Vector3 m_originalPosition;
    private float m_currentShakeTime;
    private float m_currentMagnitude;

    private void Awake()
    {
        Instance = this;
        m_originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (m_currentShakeTime > 0)
        {
            transform.localPosition = m_originalPosition + Random.insideUnitSphere * m_currentMagnitude;
            m_currentShakeTime -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = m_originalPosition;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        m_currentShakeTime = duration;
        m_currentMagnitude = magnitude;
    }

    public void Shake()
    {
        Shake(m_shakeDuration, m_shakeMagnitude);
    }
}
