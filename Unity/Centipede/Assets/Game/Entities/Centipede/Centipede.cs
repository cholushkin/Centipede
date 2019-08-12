using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class Centipede : BoardEntityBase
    {
        public class EventDestroyCentipedeSegment
        {
            public bool IsHead;
        }
        public class Segment
        {
            public Segment(Centipede centipede, Vector2Int pos, Segment nextSegment)
            {
                _centipede = centipede;
                _visualSegment = _centipede.Visual.CreateVisualSegment(nextSegment == null);
                SetBoardPosition(pos, false);
                Next = nextSegment;
                if (nextSegment != null)
                    nextSegment.Prev = this;
                Prev = null;
            }

            public void SetBoardPosition(Vector2Int pos, bool removePrev)
            {
                if (removePrev)
                    _centipede.Board.CellAccessor.Set(_boardPos, null);
                _boardPos = pos;
                _visualSegment.transform.position = _centipede.Board.ToWorldPosition(pos);
                _visualSegment.SetActive(_centipede.Board.CellAccessor
                    .IsInside(pos)); // don't draw outside the field (for spawning)
                _centipede.Board.CellAccessor.Set(pos, _centipede);
            }

            public Vector2Int GetBoardPosition()
            {
                return _boardPos;
            }

            public GameObject GetVisualSegment() // todo: VisualSegment script
            {
                return _visualSegment;
            }

            public void Disconnet()
            {
                var pSeg = Prev;
                var nSeg = Next;
                Next = Prev = null;
                if (pSeg != null)
                    pSeg.Next = null;
                if (nSeg != null)
                    nSeg.Prev = null;
            }

            public void Insert(Segment newEntry)
            {
                Segment oldPrev = Prev;
                if (oldPrev != null)
                    oldPrev.Next = newEntry;
                Prev = newEntry;
                newEntry.Next = this;
                newEntry.Prev = oldPrev;
            }

            public void Die()
            {
                _centipede.Board.CellAccessor.Set(_boardPos, null);
                Destroy(_visualSegment);
            }

            public void SetOwnerCentipede(Centipede centipede)
            {
                _centipede = centipede;
            }

            public Segment Prev; // moving to the tail
            public Segment Next; // moving to the head
            private Vector2Int _boardPos;
            private readonly GameObject _visualSegment;
            private Centipede _centipede;
        }

        public CentipedeVisual Visual;
        private Vector2Int _direction; // current direction
        private Vector2Int _lastHorizontalDirection; // last direction took in horizontal plane
        private Vector2Int _lastVerticalDirection; // last direction took in vertical plane

        private float _speed;
        private int _maxTopY; // active level area (same as for the player)
        private Segment _head;
        private Segment _tail;

        private float _currentStepCooldown;
        private int _waitCounter;
        private int _vertStepsCount;
        private static readonly int WaitStepsMax = 10;


        void Update()
        {
            // centipede thinking
            _currentStepCooldown -= Time.deltaTime;
            if (_currentStepCooldown < 0f)
            {
                _currentStepCooldown = _speed; // wait for next step

                // do step
                var dest = StepHorizOrVert();
                if (dest == _boardPosition)
                {
                    ++_waitCounter; // we are stuck, wait for a few steps and eat the way out
                    Debug.LogFormat("{0}:waiting.{1} at {2}", name, _waitCounter, _boardPosition);
                    return;
                }
                MoveOneStep(dest);
            }

            //// todo: remove me, dbg
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //    ApplyDamage(1f, _head.GetBoardPosition());
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    var pointer = _head;
            //    var cnt = 2;
            //    while (pointer != null) // find injured segment and split
            //    {
            //        ++cnt;
            //        pointer = pointer.Prev;
            //    }
            //    cnt = cnt / 2;
            //    pointer = _head;
            //    while (pointer != null) // find injured segment and split
            //    {
            //        --cnt;
            //        if (cnt < 0)
            //        {
            //            Debug.Log("Debug damage in the midle");
            //            ApplyDamage(1f, pointer.GetBoardPosition());
            //            break;
            //        }
            //        pointer = pointer.Prev;
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //    ApplyDamage(1f, _tail.GetBoardPosition());
        }

        public void ApplyDamage(float damage, Vector2Int boardPosition)
        {
            //Debug.LogFormat("Damage {0} ->SPLITTING ", boardPosition);
            //DbgPrintState();
            Split(boardPosition);

            // spawn mushroom
            StateGameplay.Instance.Level.SpawnMushroom(boardPosition);
        }

        public void Split(Vector2Int boardPosition)
        {
            var pointer = _head;
            //Debug.Log("looking for>>>" + boardPosition);
            while (pointer != null) // find injured segment and split
            {
                //Debug.Log(pointer.GetBoardPosition());
                if (pointer.GetBoardPosition() == boardPosition) // found
                {
                    //Debug.Log("found");
                    var tailOfSnake2 = _tail;
                    _tail = pointer.Next;
                    var headOfSnake2 = pointer.Prev;
                    pointer.Disconnet();
                    pointer.Die();

                    if (headOfSnake2 != null)
                        StateGameplay.Instance.Level.SpawnCentipede(headOfSnake2, tailOfSnake2, _lastVerticalDirection);

                    GlobalEventAggregator.EventAggregator.Publish(new EventDestroyCentipedeSegment { IsHead = _tail == null });

                    if (_tail == null) // split point is the head of the snake
                        Destroy(gameObject);
                    return;
                }
                pointer = pointer.Prev;
            }
            Assert.IsTrue(false);
        }

        private Vector2Int
            StepHorizOrVert() // returns next step destination coordinates or current coordinates if the centipede have to wait
        {
            var destination = _boardPosition + _lastHorizontalDirection;

            if (_vertStepsCount > 0) // change horiz direction
            {
                var newDir = _lastHorizontalDirection == Vector2Int.left ? Vector2Int.right : Vector2Int.left;
                if (Board.CellAccessor.Get(_boardPosition + newDir).CellType == GameConstants.CellType.Empty)
                    return _boardPosition + newDir;
            }

            var dc = Board.CellAccessor.Get(destination); // destination cell

            if (dc.CellType == GameConstants.CellType.Empty)
                return destination;

            if (dc.CellType == GameConstants.CellType.Player)
            {
                Eat(destination);
                return destination;
            }

            var vertDest = StepVert();
            if (vertDest == _boardPosition
            ) // Same position == wait. Is there on horiz plane something that we can do to avoid waiting
            {
                //Debug.Log("avoid waiting? >>>");
                if (dc.CellType == GameConstants.CellType.Mushroom) // mushroom on the way
                {
                    Eat(destination);
                    return destination;
                }
                // todo:
                // stuck: mushroom on left or on right, but we are moving vertically (could happen on the edge of the active zone)
                //if (_waitCounter >= WaitStepsMax) // we wait too long do something more dangerous
                // eat sentipede (split)
                // eat spider

            }
            else // moved to vert direction successfully
            {
                return vertDest;
            }
            return _boardPosition;
        }

        private Vector2Int StepVert()
        {
            var destination = _boardPosition + _lastVerticalDirection;

            // active area violation process 
            if (destination.y > _maxTopY && _lastVerticalDirection == Vector2Int.up)
                destination = _boardPosition + Vector2Int.down;
            if (destination.y == -1 && _lastVerticalDirection == Vector2Int.down)
                destination = _boardPosition + Vector2Int.up;

            var dc = Board.CellAccessor.Get(destination); // destination cell

            if (dc.CellType == GameConstants.CellType.Empty)
            {
                return destination;
            }

            if (dc.CellType == GameConstants.CellType.Mushroom || dc.CellType == GameConstants.CellType.Player)
            {
                Eat(destination);
                return destination;
            }
            return _boardPosition;
        }

        private void MoveOneStep(Vector2Int dest)
        {
            Assert.IsTrue(Board.CellAccessor.Get(dest).CellType == GameConstants.CellType.Empty);
            var delta = dest - _boardPosition;
            if (delta == Vector2Int.down || delta == Vector2Int.up) // moving vertically
            {
                _lastVerticalDirection = delta;
                ++_vertStepsCount;
            }
            else // moving horizotally
            {
                _vertStepsCount = 0;
                _lastHorizontalDirection = delta;
            }
            _direction = delta;
            _waitCounter = 0;
            SetBoardPosition(dest);
        }

        private void Eat(Vector2Int dest)
        {
            var cell = Board.CellAccessor.Get(dest);
            Assert.IsTrue(cell.CellType != GameConstants.CellType.Empty);
            Debug.LogFormat("{0}: ate {1} at {2}", name, cell.CellType, dest);
            cell.Entity.Remove();
        }

        public void Init(float speed, Segment head, Segment tail, BoardController board, Vector2Int moveDirection)
        {
            Assert.IsNotNull(head);
            Assert.IsNotNull(tail);
            _lastHorizontalDirection = Vector2Int.right;
            _lastVerticalDirection = moveDirection;
            _vertStepsCount = 0;
            Board = board;
            _direction = Vector2Int.down;
            _speed = speed;
            _currentStepCooldown = _speed;

            // reuse segments of host snake
            _boardPosition = head.GetBoardPosition();
            _head = head;
            _tail = tail;
            _head.GetVisualSegment().GetComponent<SpriteRenderer>().sprite =
                Visual.MultiSprite.GetSprite(0); // set head sprite

            // set new owner to the board + change parent of visual segments
            var pointer = _head;
            while (pointer != null)
            {
                pointer.SetOwnerCentipede(this);
                pointer.SetBoardPosition(pointer.GetBoardPosition(), false);
                pointer.GetVisualSegment().transform.SetParent(Visual.transform);
                pointer = pointer.Prev;
            }
        }

        public void Init(float speed, int segmentsAmount, Vector2Int pos, BoardController board)
        {
            _lastHorizontalDirection = Vector2Int.right;
            _lastVerticalDirection = Vector2Int.down;
            _vertStepsCount = 0;
            Board = board;
            _direction = Vector2Int.down;
            _speed = speed;
            _currentStepCooldown = _speed;
            CreateSegments(segmentsAmount, pos);
        }

        public void SetActiveArea(int maxTopY)
        {
            _maxTopY = maxTopY;
        }

        private void CreateSegments(int segmentsAmount, Vector2Int headPosition)
        {
            Segment segment = null;
            for (int i = 0; i < segmentsAmount; i++)
            {
                segment = new Segment(this, headPosition + Vector2Int.up * i, segment);
                if (i == 0)
                    _head = segment;
                if (i == segmentsAmount - 1)
                    _tail = segment;
            }
            _boardPosition = headPosition;
        }


        #region BoardEntityBase
        public override void Remove()
        {
            var pointer = _head;
            while (pointer != null)
            {
                Board.CellAccessor.Set(pointer.GetBoardPosition(), null);
                pointer = pointer.Prev;
            }
            Destroy(gameObject);
        }

        public override GameConstants.CellType GetCellType()
        {
            return GameConstants.CellType.Centipede;
        }

        public override void SetBoardPosition(Vector2Int pos, bool clearPrevPosition = true)
        {
            // we don't move whole snake, just manipulate head and tail
            Assert.IsTrue(Board.CellAccessor.Get(pos).CellType == GameConstants.CellType.Empty);
            Assert.IsTrue(clearPrevPosition);

            // set new pos to head segment
            _head.SetBoardPosition(pos, clearPrevPosition);
            Board.CellAccessor.Set(pos, this);

            // set tail segment to old head position
            if (_head != _tail)
            {
                var twoSegmented = (_head.Prev == _tail);
                var newTail = _tail.Next;
                if (!twoSegmented)
                {
                    _tail.Disconnet();
                    _head.Insert(_tail);
                }
                _tail.SetBoardPosition(_boardPosition, clearPrevPosition);
                //Board.CellAccessor.Set(_boardPosition, this);
                if (!twoSegmented)
                    _tail = newTail;
            }

            _boardPosition = pos;
        }
        #endregion


        //[ContextMenu("DbgPrintState")]
        //public void DbgPrintState()
        //{
        //    var pointer = _head;
        //    var str = "";
        //    var cnt = 0;
        //    while (pointer != null) // find injured segment and split
        //    {
        //        str += pointer.GetBoardPosition().ToString() + ", ";
        //        cnt++;
        //        pointer = pointer.Prev;
        //    }
        //    Debug.LogFormat("Centipede{0}:({1}) {2}", cnt, GetInstanceID(), str);
        //}
    }
}