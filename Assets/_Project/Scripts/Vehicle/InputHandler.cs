using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    [Header("Input Axes")]
    [SerializeField] private string m_horizontalAxis = "Horizontal";
    [SerializeField] private string m_verticalAxis = "Vertical";
    [SerializeField] private string m_brakeAxis = "Jump";

    public float SteeringInput { get; private set; }
    public float ThrottleInput { get; private set; }
    public bool BrakeInput { get; private set; }
    public bool HandbrakeInput { get; private set; }
    public bool ResetInput { get; private set; }
    public bool PauseInput { get; private set; }

    private bool m_resetPressed;
    private bool m_pausePressed;

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
        GatherInput();
    }

    private void GatherInput()
    {
        SteeringInput = Input.GetAxisRaw(m_horizontalAxis);
        ThrottleInput = Input.GetAxisRaw(m_verticalAxis);
        
        BrakeInput = Input.GetAxis(m_brakeAxis) > 0.1f;
        HandbrakeInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        bool resetThisFrame = Input.GetKeyDown(KeyCode.R);
        if (resetThisFrame && !m_resetPressed)
        {
            ResetInput = true;
            m_resetPressed = true;
        }
        else
        {
            ResetInput = false;
        }

        if (!resetThisFrame)
        {
            m_resetPressed = false;
        }

        bool pauseThisFrame = Input.GetKeyDown(KeyCode.Escape);
        if (pauseThisFrame && !m_pausePressed)
        {
            PauseInput = true;
            m_pausePressed = true;
        }
        else
        {
            PauseInput = false;
        }

        if (!pauseThisFrame)
        {
            m_pausePressed = false;
        }
    }

    public void ResetFrameInputs()
    {
        ResetInput = false;
        PauseInput = false;
    }
}
