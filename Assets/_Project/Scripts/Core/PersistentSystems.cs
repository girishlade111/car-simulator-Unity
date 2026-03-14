using UnityEngine;

public class PersistentSystems : MonoBehaviour
{
    private void Awake()
    {
        var bootstrap = FindObjectOfType<Bootstrap>();
        if (bootstrap == null)
        {
            gameObject.AddComponent<Bootstrap>();
        }

        var gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            gameObject.AddComponent<GameManager>();
        }

        var inputHandler = FindObjectOfType<InputHandler>();
        if (inputHandler == null)
        {
            gameObject.AddComponent<InputHandler>();
        }
    }
}
