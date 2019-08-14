using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Utils
{
    public class SimpleWaveProcessor<T> where T : IComparable<T>
    {
        public delegate bool WallFunction(T val);
        private T DefFuncWallValue;
        private const sbyte NotVisited = -1;
        private const sbyte Wall = -2;
        private WallFunction WallFunc;
        private sbyte[,] Data;
        private readonly T[,] Map;

        private int MapWidth;
        private int MapHeight;
        public List<Wave> Waves { get; private set; }
        private sbyte CurWaveIndex;
        private Vector2Int _target;

        public class Wave
        {
            public List<Vector2Int> Cells = new List<Vector2Int>();

            public bool IsEmpty()
            {
                return Cells.Count == 0;
            }
        }

        public SimpleWaveProcessor(T[,] map, WallFunction wf, Vector2Int? epicentre = null)
        {
            // map
            WallFunc = wf;
            Map = map;

            MapWidth = map.GetLength(0);
            MapHeight = map.GetLength(1);
            Data = new sbyte[MapWidth, MapHeight];
            Clear();

            if (epicentre != null)
                ComputeWaves(epicentre.Value);
        }

        public SimpleWaveProcessor(T[,] map, T defFuncWallValue, Vector2Int? epicentre = null)
        {
            // map
            Assert.IsNotNull(map, "SimpleWaveProcessor: passed map shouldn't be null");
            WallFunc = DefaultWallFunction;
            DefFuncWallValue = defFuncWallValue;
            Map = map;

            MapWidth = map.GetLength(0);
            MapHeight = map.GetLength(1);
            Data = new sbyte[MapWidth, MapHeight];
            Clear();

            if (epicentre != null)
                ComputeWaves(epicentre.Value);
        }

        public void SetWallFunction(WallFunction newWallFunction)
        {
            WallFunc = newWallFunction;
        }

        private bool DefaultWallFunction(T val)
        {
            return val.CompareTo(DefFuncWallValue) == 0;
        }

        public void SetTarget(Vector2Int target)
        {
            _target = target;
        }

        private Vector2Int _pbPointer; // path builder pointer
        public List<Vector2Int> GetPath()
        {
            List<Vector2Int> path = new List<Vector2Int>();
            _pbPointer = _target;
            var isAdded = true;
            while (isAdded)
            {
                var curNodeValue = Data[_pbPointer.x, _pbPointer.y];
                isAdded = false;
                for (sbyte i = 0; i < 4; ++i)
                {
                    var tp = _pbPointer + dirs[i];
                    if (IsInsideMap(tp) && !IsValue(tp, Wall) && Data[tp.x, tp.y] < curNodeValue)
                    {
                        _pbPointer = tp;
                        isAdded = true;
                        path.Add(_pbPointer);
                        break;
                    }
                }
            }
            return path;
        }

        public void Clear()
        {
            CurWaveIndex = 0;
            Waves = new List<Wave>();
            for (int y = 0; y < MapHeight; ++y)
                for (int x = 0; x < MapWidth; ++x)
                    Data[x, y] = WallFunc(Map[x, y]) ? Wall : NotVisited;
        }

        public void ComputeWaves(Vector2Int epicentre)
        {
            // add first wave
            Wave wave = new Wave();
            Data[epicentre.x, epicentre.y] = 0;
            wave.Cells.Add(epicentre);

            // compute waves
            do
            {
                Waves.Add(wave);
                wave = PrepareWave(wave);
            } while (!wave.IsEmpty());
        }

        private Wave PrepareWave(Wave prevWave)
        {
            Wave curWave = new Wave();
            Assert.IsTrue(CurWaveIndex < sbyte.MaxValue, "the field is too big (consider Data as int array)");
            ++CurWaveIndex;

            foreach (var waveCell in prevWave.Cells)
            {
                var neig = GetNotVisitedNeighbours(waveCell);
                foreach (var neigCell in neig)
                {
                    if (neigCell == _target)
                        _pbPointer = neigCell;
                    curWave.Cells.Add(neigCell);
                    Data[neigCell.x, neigCell.y] = CurWaveIndex;
                }
            }
            return curWave;
        }

        private static readonly Vector2Int[] dirs =
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1)
        };

        private List<Vector2Int> GetNotVisitedNeighbours(Vector2Int cell)
        {
            var result = new List<Vector2Int>(4);
            Vector2Int pointer = new Vector2Int();

            for (sbyte i = 0; i < 4; ++i)
            {
                pointer = cell + dirs[i];
                if (IsInsideMap(pointer) && !IsValue(pointer, Wall) && (IsValue(pointer, NotVisited)))
                    result.Add(pointer);
            }
            return result;
        }

        private bool IsValue(Vector2Int pointer, sbyte val)
        {
            return Data[pointer.x, pointer.y] == val;
        }

        private bool IsInsideMap(Vector2Int pointer)
        {
            return pointer.x >= 0 && pointer.y >= 0 && pointer.x < MapWidth && pointer.y < MapHeight;
        }
    }
}
