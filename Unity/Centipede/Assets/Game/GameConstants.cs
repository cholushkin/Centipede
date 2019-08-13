namespace Game
{
    // game rules 
    public static class GameConstants
    {
        // board
        public enum CellType : sbyte
        {
            Empty = 0,
            Undefined = 1,
            Mushroom = 2,
            Player = 3,
            Centipede = 4,
            Spider = 5
        }
        public static readonly float CellWidth = 0.1f;
        public static readonly float CellHeight = 0.1f;
        public static readonly float InitialSpawningDelay = 0.01f;
        public static readonly float LevelStartCentipedeSpawninDelay = 2f;
        public static readonly float LevelStartSpiderSpawninDelay = 4f;
        public static readonly float SpiderAttackingDuration = 0.5f;

        // player
        public static readonly byte InitialLivesAmount = 3;
    }
}