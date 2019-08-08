using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public enum CellType : sbyte
    {
        Undefined = 0,
        Empty = 1,
        Wall = 2,
        Mushroom = 3,
        Player = 4,
        Centipede = 5,
        Spider = 6
    }
}
