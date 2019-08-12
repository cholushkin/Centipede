namespace Game
{
    public static class GameConstants
    {
        public enum CellType : sbyte
        {
            Empty = 0,
            Undefined = 1,
            Mushroom = 2,
            Player = 3,
            Centipede = 4,
            Spider = 5
        }

        // player
        public static readonly byte InitialLivesAmount = 3;

        // board
        public static readonly float CellWidth = 0.1f;

        public static readonly float CellHeight = 0.1f;
        public static readonly float InitialSpawningDelay = 0.01f;

        public static readonly float LevelStartCentipedeSpawninDelay = 2f;
        public static readonly float LevelStartSpiderSpawninDelay = 4f;
        public static readonly byte SpiderKeepDistanceRadius = 1;
        public static readonly float SpiderAttackingDuration = 0.5f;
    }
}