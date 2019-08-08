using UnityEngine;

public class CellAccessor
{
    private readonly GameConstants.CellType[,] _state;
    private readonly Vector2Int BoardSize;

    public CellAccessor(Vector2Int boardSize)
    {
        BoardSize = boardSize;
        _state = new GameConstants.CellType[BoardSize.x, BoardSize.y];
    }

    public bool IsInside(int x, int y)
    {
        if (x < 0 || x >= BoardSize.x)
            return false;
        if (y < 0 || y >= BoardSize.y)
            return false;
        return true;
    }

    public bool IsInside(Vector2Int coord)
    {
        return IsInside(coord.x, coord.y);
    }

    public void Set(int x, int y, GameConstants.CellType value)
    {
        if (!IsInside(x, y))
        {
            Debug.LogWarningFormat("Trying to set value outside of the board. Coord({0},{1}), Value: {2}", x, y, value);
            return;
        }
        _state[x, y] = value;
    }


    public void Set(Vector2Int coord, GameConstants.CellType value)
    {
        Set(coord.x, coord.y, value);
    }

    public GameConstants.CellType Get(Vector2Int coord)
    {
        return Get(coord.x, coord.y);
    }

    public GameConstants.CellType Get(int x, int y)
    {
        if (!IsInside(x, y))
        {
            Debug.LogWarningFormat("Trying to get value outside of the board. Coord({0},{1}), Value: undefined", x, y);
            return GameConstants.CellType.Undefined;
        }
        return _state[x, y];
    }
}
