namespace CarSimulator
{
    public static class GameConstants
    {
        public const string DEFAULT_SCENE = "MainMenu";
        public const string OPEN_WORLD_SCENE = "OpenWorld_TestDistrict";
        public const string GARAGE_SCENE = "Garage_Test";

        public const float DEFAULT_TIME_SCALE = 1f;
        public const float PAUSED_TIME_SCALE = 0f;

        public const int SAVE_SLOT_COUNT = 10;
        public const float AUTO_SAVE_INTERVAL = 120f;

        public static class Tags
        {
            public const string Player = "Player";
            public const string Vehicle = "Vehicle";
            public const string Building = "Building";
            public const string Prop = "Prop";
            public const string Ground = "Ground";
        }

        public static class Layers
        {
            public const string Default = "Default";
            public const string Player = "Player";
            public const string Vehicle = "Vehicle";
            public const string Building = "Building";
            public const string Prop = "Prop";
            public const string Ground = "Ground";
            public const string UI = "UI";
        }
    }
}
