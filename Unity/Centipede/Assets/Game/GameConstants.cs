public static class GameConstants
{
    public enum CellType : sbyte
    {
        Empty = 0,
        Undefined = 1,
        Wall = 2,
        Mushroom = 3,
        Player = 4,
        Centipede = 5,
        Spider = 6
    }

    // player
    public static readonly byte InitialLivesAmount = 2;
    public static readonly float GodModeTimer = 2f;

    // board
    public static readonly float CellWidth = 0.09f;
    public static readonly float CellHeight = 0.1f;
    public static readonly float InitialSpawningDelay = 0.05f;
}