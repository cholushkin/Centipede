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
    public static readonly byte InitialLivesAmount = 2;
    public static readonly float GodModeTimer = 2f;

    // board
    public static readonly float CellWidth = 0.09f;
    public static readonly float CellHeight = 0.1f;
    public static readonly float InitialSpawningDelay = 0.01f;
    public static readonly float LevelStartCentipedeSpawninDelay = 2f;
}