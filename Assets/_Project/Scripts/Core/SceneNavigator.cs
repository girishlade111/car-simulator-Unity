using UnityEngine;

public static class SceneNavigator
{
    public static void GoToMainMenu()
    {
        Bootstrap.LoadScene("MainMenu");
    }

    public static void GoToOpenWorld()
    {
        Bootstrap.LoadScene("OpenWorld_TestDistrict");
    }

    public static void GoToGarage()
    {
        Bootstrap.LoadScene("Garage_Test");
    }

    public static void RestartCurrentScene()
    {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Bootstrap.LoadScene(currentScene);
    }
}
