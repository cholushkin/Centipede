using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace Game
{
    public class PlayerController : BoardEntityBase
    {
        public class EventPlayerDie
        {
        }

        public enum State
        {
            Alive,
            Dead
        }

        public Weapon Weapon;
        public PlayerVisual Visual;
        public bool IsSlideModeMovement;
        public float Speed;
        public Follower Follower;
        public float DeathDuration;

        private Vector3 _positionPointer;
        private int _maxTopY;
        private State _currentState;
        private float _deathTimer;
        private Vector3 _inputMove;
        private bool _inputIsShoot;
        private bool _isInjuredDuringMovement;

        public void Init(Vector2Int boardPos, BoardController board, BalanceConfig config)
        {
            _currentState = State.Alive;
            Board = board;
            Weapon.Board = Board;
            Weapon.BulletSpeed = config.PlayerBulletSpeed;
            Speed = config.PlayerSpeed;
            SetBoardPosition(boardPos, false);
            transform.position = Board.ToWorldPosition(boardPos);
            SetActiveArea(Mathf.RoundToInt(config.GridSize.y * config.ActiveAreaOffsetPercent));
            _positionPointer = transform.position;
        }

        void Update()
        {
            // process death
            if (_currentState == State.Dead)
            {
                _deathTimer -= Time.deltaTime;
                if (_deathTimer < 0f)
                {
                    GlobalEventAggregator.EventAggregator.Publish(new EventPlayerDie());
                    Destroy(gameObject);
                }
            }
            else if (_currentState == State.Alive)
            {
                ProcessInput();

                if (_inputIsShoot)
                {
                    Assert.IsNotNull(Weapon);
                    Weapon.Shoot(Vector3.up);
                }

                _isInjuredDuringMovement = false;
                var isMoved = false;
                if (IsSlideModeMovement)
                    isMoved = ProcessMovement(_inputMove) || ProcessMovement(new Vector3(0, _inputMove.y, 0)) ||
                              ProcessMovement(new Vector3(_inputMove.x, 0, 0));
                else
                    isMoved = ProcessMovement(_inputMove);

                //// debug seppuku // todo: remove me
                //if (Input.GetKeyDown(KeyCode.S))
                //    _isInjuredDuringMovement = true;

                if (_isInjuredDuringMovement)
                    Remove();
            }
        }

        private void ProcessInput()
        {
            // shoot
            _inputIsShoot = Input.GetKey(KeyCode.Space);

            // move
            _inputMove = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftArrow))
                _inputMove += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow))
                _inputMove += Vector3.right;
            if (Input.GetKey(KeyCode.UpArrow))
                _inputMove += Vector3.up;
            if (Input.GetKey(KeyCode.DownArrow))
                _inputMove += Vector3.down;
        }


        private bool ProcessMovement(Vector3 offset)
        {
            var newPositionPointer = _positionPointer + offset * Speed * Time.deltaTime;
            var newBoardPos = Board.ToBoardPosition(newPositionPointer);
            var cell = Board.CellAccessor.Get(newBoardPos);

            // restrict by screen edges
            var isOut = cell.CellType == GameConstants.CellType.Undefined;
            if (isOut)
                return false;

            // restrict by _maxTopY
            if (newBoardPos.y >= _maxTopY)
                return false;

            // mushroom?
            if (cell.CellType == GameConstants.CellType.Mushroom)
                return false;

            // enemy?
            if (cell.CellType == GameConstants.CellType.Centipede || cell.CellType == GameConstants.CellType.Spider)
            {
                _isInjuredDuringMovement = true;
                return false;
            }

            SetBoardPosition(newBoardPos);

            _positionPointer = newPositionPointer;
            Follower.Follow(_positionPointer);
            return true;
        }

        private void SetActiveArea(int maxTopY)
        {
            _maxTopY = maxTopY;
        }

        #region BoardEntityBase
        public override GameConstants.CellType GetCellType()
        {
            return GameConstants.CellType.Player;
        }

        public override void Remove()
        {
            if (_currentState == State.Dead)
                return;
            Debug.Log("Dead");
            _currentState = State.Dead;
            Visual.PlayDead();
            Board.CellAccessor.Set(_boardPosition, null);
            _deathTimer = DeathDuration;
        }
        #endregion
    }
}