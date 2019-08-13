using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace Game
{
    public class Spider : BoardEntityBase
    {
        public class EventSpiderDied
        {
        }

        private static SimpleWaveProcessor<int> _waveProcessor;
        private static int[,] _map;
        private static readonly int Wall = 1;

        private float _stepDelay;
        private Vector2Int _prevTargetPos = new Vector2Int(-1, -1);
        private Vector2Int _nextStep;
        private List<Vector2Int> _path;
        private float _currentStepCooldown;
        private float _attackinTimer;

        public void Init(float stepDelay, Vector2Int boardPos, BoardController board)
        {
            Assert.IsTrue(stepDelay != 0f);
            Board = board;
            _stepDelay = stepDelay;
            _currentStepCooldown = _stepDelay;
            SetBoardPosition(boardPos, false);
        }

        void Awake()
        {
            var currentMapSize = StateGameplay.Instance.Level.Balance.GridSize;
            _map = new int[currentMapSize.x, currentMapSize.y];
            _waveProcessor = new SimpleWaveProcessor<int>(_map, val => val == Wall);
        }

        void Update()
        {
            // move to target
            _currentStepCooldown -= Time.deltaTime;
            if (_currentStepCooldown < 0f)
            {
                _currentStepCooldown = _stepDelay; // wait for next step
                SetBoardPosition(_nextStep);
                _nextStep = GetNextStep();
            }

            // get player
            var player = StateGameplay.Instance.Level.Player;
            if (player.CurrentState == PlayerController.State.Dead)
                return;

            // attacking the player
            var toPlayerDist = (player.GetBoardPosition() - _boardPosition).magnitude;
            _attackinTimer = toPlayerDist > 1f ? 0f : _attackinTimer + Time.deltaTime;
            if (_attackinTimer >= GameConstants.SpiderAttackingDuration)
                player.Remove();

            // calculate path
            if (player.GetBoardPosition() == _prevTargetPos)
                return;
            _prevTargetPos = player.GetBoardPosition();
            _path = CalculatePath();
            GetNextStep(); // remove one node from the path
            _nextStep = GetNextStep();
        }

        private Vector2Int GetNextStep()
        {
            if (_path == null || _path.Count == 0)
                return _boardPosition;
            var step = _path[_path.Count - 1];
            _path.RemoveAt(_path.Count - 1);
            return step;
        }

        public List<Vector2Int> CalculatePath()
        {
            UpdateMap();
            _waveProcessor.Clear();
            _waveProcessor.SetTarget(_prevTargetPos);
            _waveProcessor.ComputeWaves(GetBoardPosition());
            return _waveProcessor.GetPath();
        }

        private void UpdateMap()
        {
            for (int x = 0; x < Board.CellAccessor.BoardSize.x; ++x)
                for (int y = 0; y < Board.CellAccessor.BoardSize.y; ++y)
                {
                    _map[x, y] = 0;
                    var cell = Board.CellAccessor.Get(x, y);
                    if (cell.CellType != GameConstants.CellType.Empty && cell.CellType != GameConstants.CellType.Player)
                        _map[x, y] = Wall;
                }
        }

        public void ApplyDamage(float damage)
        {
            GlobalEventAggregator.EventAggregator.Publish(new EventSpiderDied());
            Remove();
        }

        #region BoardEntityBase
        public override GameConstants.CellType GetCellType()
        {
            return GameConstants.CellType.Spider;
        }

        public override void SetBoardPosition(Vector2Int pos, bool clearPrevPosition = true)
        {
            base.SetBoardPosition(pos, clearPrevPosition);
            transform.position = Board.ToWorldPosition(pos);
        }

        public override void Remove()
        {
            Board.CellAccessor.Set(_boardPosition, null);
            Destroy(gameObject);
        }
        #endregion
    }
}