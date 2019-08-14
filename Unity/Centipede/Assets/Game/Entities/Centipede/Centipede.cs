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

        public class EventCentipedeKilled
        {
        }

        public class Segment
        {
            internal Segment(Centipede centipede, Vector2Int pos, Segment nextSegment)
            {
                _centipede = centipede;
                _visualSegment = _centipede.Visual.CreateVisualSegment(nextSegment == null);
                SetBoardPosition(pos, false);
                Next = nextSegment;
                if (nextSegment != null)
                    nextSegment.Prev = this;
                Prev = null;
            }

            internal void SetBoardPosition(Vector2Int pos, bool removePrev)
            {
                if (removePrev)
                    _centipede.Board.CellAccessor.Set(_boardPos, null);
                _boardPos = pos;
                _visualSegment.transform.position = _centipede.Board.ToWorldPosition(pos);
                _visualSegment.SetActive(_centipede.Board.CellAccessor
                    .IsInside(pos)); // don't draw outside the field (for spawning)
                _centipede.Board.CellAccessor.Set(pos, _centipede);
            }

            internal Vector2Int GetBoardPosition()
            {
                return _boardPos;
            }

            internal GameObject GetVisualSegment()
            {
                return _visualSegment;
            }

            internal void Disconnet()
            {
                var pSeg = Prev;
                var nSeg = Next;
                Next = Prev = null;
                if (pSeg != null)
                    pSeg.Next = null;
                if (nSeg != null)
                    nSeg.Prev = null;
            }

            internal void Insert(Segment newEntry)
            {
                Segment oldPrev = Prev;
                if (oldPrev != null)
                    oldPrev.Next = newEntry;
                Prev = newEntry;
                newEntry.Next = this;
                newEntry.Prev = oldPrev;
            }

            internal void Die()
            {
                _centipede.Board.CellAccessor.Set(_boardPos, null);
                Destroy(_visualSegment);
            }

            internal void SetOwnerCentipede(Centipede centipede)
            {
                _centipede = centipede;
            }

            public Segment Prev { get; private set; } // moving to the tail
            public Segment Next { get; private set; } // moving to the head
            private Vector2Int _boardPos;
            private readonly GameObject _visualSegment;
            private Centipede _centipede;
        }

        public CentipedeVisual Visual;
        private Vector2Int _direction; // current direction
        private Vector2Int _lastHorizontalDirection; // last direction took in horizontal plane
        private Vector2Int _lastVerticalDirection; // last direction took in vertical plane

        private float _stepDelay;
        private int _maxTopY; // active level area (same as for the player)
        private Segment _head;
        private Segment _tail;

        private float _currentStepCooldown;
        private int _waitCounter;
        private int _vertStepsCount;
        private bool _isKilled;
        private static readonly int WaitStepsMax = 8; // steps amount in the stuck state before trying to resolve it

        private void Update()
        {
            // centipede thinking
            _currentStepCooldown -= Time.deltaTime;
            if (_currentStepCooldown < 0f)
            {
                _currentStepCooldown = _stepDelay; // wait for next step

                // do step
                var dest = StepHorizOrVert();
                if (dest == _boardPosition)
                {
                    ++_waitCounter; // we are stuck, wait for a several steps and eat the way out
                    return;
                }
                MoveOneStep(dest);
            }
        }

        public void OnDestroy()
        {
            if (_isKilled)
                GlobalEventAggregator.EventAggregator.Publish(new EventCentipedeKilled());
        }

        public void Init(float speed, int segmentsAmount, Vector2Int pos, BoardController board)
        {
            _lastHorizontalDirection = Vector2Int.right;
            _lastVerticalDirection = Vector2Int.down;
            _vertStepsCount = 0;
            Board = board;
            _direction = Vector2Int.down;
            _stepDelay = speed;
            _currentStepCooldown = _stepDelay;
            CreateSegments(segmentsAmount, pos);
        }

        // for split init
        public void Init(float speed, Segment head, Segment tail, BoardController board, Vector2Int moveDirection)
        {
            Assert.IsNotNull(head);
            Assert.IsNotNull(tail);
            _lastHorizontalDirection = Vector2Int.right;
            _lastVerticalDirection = moveDirection;
            _vertStepsCount = 0;
            Board = board;
            _direction = Vector2Int.down;
            _stepDelay = speed;
            _currentStepCooldown = _stepDelay;

            // reuse segments of the host snake
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

        public void ApplyDamage(float damage, Vector2Int boardPosition)
        {
            Split(boardPosition);
            StateGameplay.Instance.Level.SpawnMushroom(boardPosition);
        }

        private void Split(Vector2Int boardPosition)
        {
            var pointer = _head;
            while (pointer != null) // find split point segment and split
            {
                if (pointer.GetBoardPosition() == boardPosition) // found
                {
                    var tailOfSnake2 = _tail;
                    _tail = pointer.Next;
                    var headOfSnake2 = pointer.Prev;
                    pointer.Disconnet();
                    pointer.Die();

                    if (headOfSnake2 != null)
                        StateGameplay.Instance.Level.SpawnCentipede(headOfSnake2, tailOfSnake2, _lastVerticalDirection);

                    if (_tail == null) // split point is the head of the snake
                    {
                        Destroy(gameObject);
                        _isKilled = true; // killed but not destroyed by purging
                    }

                    GlobalEventAggregator.EventAggregator.Publish(new EventDestroyCentipedeSegment { IsHead = _tail == null });
                    return;
                }
                pointer = pointer.Prev;
            }
            Assert.IsTrue(false);
        }

        private void Eat(Vector2Int dest)
        {
            var cell = Board.CellAccessor.Get(dest);
            Assert.IsTrue(cell.CellType != GameConstants.CellType.Empty);
            cell.Entity.Remove();
        }

        #region AI
        private Vector2Int StepHorizOrVert() // returns next step destination coordinates or current coordinates if the centipede have to wait
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
            if (vertDest == _boardPosition) // Same position == wait. Is there on horiz plane something that we can do to avoid waiting
            {
                // avoid waiting?
                if (dc.CellType == GameConstants.CellType.Mushroom) // mushroom on the way
                {
                    Eat(destination);
                    return destination;
                }
                if (_waitCounter > WaitStepsMax)
                {
                    return ResolveStuck();
                }
            }
            else // moved to vert direction successfully
                return vertDest;

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

        private readonly Vector2Int[] _directions = { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
        private Vector2Int ResolveStuck()
        {
            for (int i = 0; i < 4; ++i)
            {
                var checkPos = _boardPosition + _directions[i];
                if (checkPos != _boardPosition - _direction) // if it is not oposite direction
                {
                    var cell = Board.CellAccessor.Get(checkPos);
                    if (cell.CellType == GameConstants.CellType.Centipede) // we eat other centipede or self o_O
                    {
                        Debug.Log("EAT centipede in STUCK");
                        var cent = cell.Entity as Centipede;
                        Assert.IsNotNull(cent);
                        cent.Split(checkPos);
                        return _boardPosition;
                    }
                    if (cell.CellType == GameConstants.CellType.Mushroom)
                    {
                        Debug.Log("EAT Mushrom in STUCK");
                        Eat(checkPos);
                        return checkPos;
                    }
                    // also we could eat spider here...
                }
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
        #endregion

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
                var twoSegmented = _head.Prev == _tail;
                var newTail = _tail.Next;
                if (!twoSegmented)
                {
                    _tail.Disconnet();
                    _head.Insert(_tail);
                }
                _tail.SetBoardPosition(_boardPosition, clearPrevPosition);
                if (!twoSegmented)
                    _tail = newTail;
            }
            _boardPosition = pos;
        }
        #endregion
    }
}