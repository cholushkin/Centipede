using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class Laser : MonoBehaviour
    {
        public BoardController Board;

        public float Speed;
        public float Damage;

        private Vector2Int _boardPosition;

        public void Init(BoardController board, Vector3 pos)
        {
            Board = board;
            transform.position = pos;
            _boardPosition = Board.ToBoardPosition(transform.position);
        }

        private void Update()
        {
            var delta = Speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y + delta, transform.position.z);
            var prevBoardPos = _boardPosition;
            _boardPosition = Board.ToBoardPosition(transform.position);

            // too fast bullet handling, to avoid cells skipping
            var oneFrameDistance = (_boardPosition - prevBoardPos).magnitude; // one frame distance in cells
            for (int i = 0; i < oneFrameDistance; ++i) // kinda raycast
                if (ProcessCollisions(prevBoardPos + Vector2Int.up * (i + 1)))
                    return;
        }

        private bool ProcessCollisions(Vector2Int checkCoord)
        {
            var cell = Board.CellAccessor.Get(checkCoord);

            switch (cell.CellType)
            {
                case GameConstants.CellType.Centipede:
                    {
                        var centipede = cell.Entity as Centipede;
                        Assert.IsNotNull(cell.Entity);
                        Assert.IsNotNull(centipede, cell.Entity.GetType().ToString());
                        //centipede.DbgPrintState();
                        centipede.ApplyDamage(Damage, checkCoord);
                        Die();
                        return true;
                    }
                case GameConstants.CellType.Mushroom:
                    {
                        var mushroom = cell.Entity as Mushroom;
                        Assert.IsNotNull(mushroom);
                        mushroom.ApplyDamage(Damage);
                        Die();
                        return true;
                    }
                case GameConstants.CellType.Spider:
                    {
                        var spider = cell.Entity as Spider;
                        Assert.IsNotNull(spider);
                        spider.ApplyDamage(Damage);
                        Die();
                        return true;
                    }
                case GameConstants.CellType.Undefined:
                    {
                        Die();
                        return true;
                    }
            }
            return false;
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}