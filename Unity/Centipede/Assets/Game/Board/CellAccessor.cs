using UnityEngine;

namespace Game
{
    public class CellAccessor
    {
        public struct Cell
        {
            public GameConstants.CellType CellType;
            public IBoardEntity Entity;
        }

        private readonly Cell[,] _cells;
        public readonly Vector2Int BoardSize;

        private static readonly Cell CellUndefined =
            new Cell { CellType = GameConstants.CellType.Undefined, Entity = null };

        public CellAccessor(Vector2Int boardSize)
        {
            BoardSize = boardSize;
            _cells = new Cell[BoardSize.x, BoardSize.y];
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

        public void Set(int x, int y, IBoardEntity entity)
        {
            if (!IsInside(x, y))
            {
                //Debug.LogWarningFormat("Trying to set value outside of the board. Coord({0},{1}), Value: {2}", x, y, entity?.GetCellType().ToString()??"null");
                return;
            }
            _cells[x, y].Entity = entity;
            _cells[x, y].CellType = entity?.GetCellType() ?? GameConstants.CellType.Empty;
        }


        public void Set(Vector2Int coord, IBoardEntity entity)
        {
            Set(coord.x, coord.y, entity);
        }

        public Cell Get(Vector2Int coord)
        {
            return Get(coord.x, coord.y);
        }

        public Cell Get(int x, int y)
        {
            if (!IsInside(x, y))
            {
                //Debug.LogWarningFormat("Trying to get value outside of the board. Coord({0},{1}), Value: undefined", x, y);
                return CellUndefined;
            }
            return _cells[x, y];
        }
    }
}