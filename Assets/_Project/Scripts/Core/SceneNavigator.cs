using CarSimulator.Runtime;
using CarSimulator.Bootstrap;

namespace CarSimulator.Core
{
    public static class SceneNavigator
    {
        public static void GoToMainMenu()
        {
            SceneLoader.Load(GameConstants.DEFAULT_SCENE);
        }

        public static void GoToOpenWorld()
        {
            SceneLoader.Load(GameConstants.OPEN_WORLD_SCENE);
        }

        public static void GoToGarage()
        {
            SceneLoader.Load(GameConstants.GARAGE_SCENE);
        }

        public static void Restart()
        {
            SceneLoader.ReloadCurrent();
        }

        public static void Load(string sceneName)
        {
            SceneLoader.Load(sceneName);
        }
    }
}
