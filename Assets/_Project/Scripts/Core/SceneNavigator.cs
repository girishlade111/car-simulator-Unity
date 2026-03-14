public static class SceneNavigator
{
    public static void GoToMainMenu()
    {
        Bootstrap.GoToMainMenu();
    }

    public static void GoToOpenWorld()
    {
        Bootstrap.GoToOpenWorld();
    }

    public static void GoToGarage()
    {
        Bootstrap.GoToGarage();
    }

    public static void RestartCurrentScene()
    {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Bootstrap.LoadScene(currentScene);
    }

    public static void LoadScene(string sceneName)
    {
        Bootstrap.LoadScene(sceneName);
    }

    public static void SaveGame(int slotIndex, string saveName)
    {
        Bootstrap.SaveGame(slotIndex, saveName);
    }

    public static void LoadGame(int slotIndex)
    {
        Bootstrap.LoadGame(slotIndex);
    }

    public static void NewGame()
    {
        Bootstrap.NewGame();
    }
}
