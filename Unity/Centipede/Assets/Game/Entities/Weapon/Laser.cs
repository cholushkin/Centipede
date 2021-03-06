using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class Laser : MonoBehaviour
    {
        public float Damage;
        public GameObject[] PrefabExplosions;
        public float Speed { get; set; }

        private Vector2Int _boardPosition;
        private BoardController _board;

        public void Init(BoardController board, Vector3 pos, float speed)
        {
            _board = board;
            transform.position = pos;
            _boardPosition = _board.ToBoardPosition(transform.position);
            Speed = speed;
        }

        private void Update()
        {
            var delta = Speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y + delta, transform.position.z);
            var prevBoardPos = _boardPosition;
            _boardPosition = _board.ToBoardPosition(transform.position);

            // too fast bullet handling, to avoid cells skipping
            var oneFrameDistance = (_boardPosition - prevBoardPos).magnitude; // one frame distance in cells
            for (int i = 0; i < oneFrameDistance; ++i) // kinda raycast
                if (ProcessCollisions(prevBoardPos + Vector2Int.up * (i + 1)))
                    return;
        }

        private bool ProcessCollisions(Vector2Int checkCoord)
        {
            var cell = _board.CellAccessor.Get(checkCoord);

            switch (cell.CellType)
            {
                case GameConstants.CellType.Centipede:
                    {
                        var centipede = cell.Entity as Centipede;
                        Assert.IsNotNull(cell.Entity);
                        Assert.IsNotNull(centipede, cell.Entity.GetType().ToString());
                        centipede.ApplyDamage(Damage, checkCoord);
                        DoExplosion(1, _board.ToWorldPosition(checkCoord));
                        Die();
                        return true;
                    }
                case GameConstants.CellType.Mushroom:
                    {
                        var mushroom = cell.Entity as Mushroom;
                        Assert.IsNotNull(mushroom);
                        mushroom.ApplyDamage(Damage);
                        DoExplosion(0, _board.ToWorldPosition(checkCoord));
                        Die();
                        return true;
                    }
                case GameConstants.CellType.Spider:
                    {
                        var spider = cell.Entity as Spider;
                        Assert.IsNotNull(spider);
                        spider.ApplyDamage(Damage);
                        DoExplosion(2, _board.ToWorldPosition(checkCoord));
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

        private void DoExplosion(int effectIndex, Vector3 at)
        {
            var explosion = Instantiate(PrefabExplosions[effectIndex]);
            explosion.transform.position = at;
            Destroy(explosion, 2f);
        }
    }
}