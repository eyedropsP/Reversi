using System;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Struct;
using UniRx;
using UnityEngine;

namespace NormalReversi.Models.Manager
{
    public class GridManager : MonoBehaviour, IGridManager
    {
        [SerializeField] private GameObject _gridPrefab;
        [SerializeField] private SpriteRenderer _gridBackgroundPrefab;
        [SerializeField] private GameObject _piecePrefab;

        private readonly IntReactiveProperty _blackPieceCount = new IntReactiveProperty(0);
        private readonly IntReactiveProperty _whitePieceCount = new IntReactiveProperty(0);
        private readonly IntReactiveProperty _canPutGridCount = new IntReactiveProperty(0);
        public IReadOnlyReactiveProperty<int> BlackPieceCount => _blackPieceCount;
        public IReadOnlyReactiveProperty<int> WhitePieceCount => _whitePieceCount;
        public IReadOnlyReactiveProperty<int> CanPutGridCount => _canPutGridCount;
        public const int GridCount = 64;

        private const int BoardSize = 8;
        private const float EndPoint = 3.85f;
        private const float Interval = 1.1f;
        private readonly IGridData[,] _gridDatas = new IGridData[BoardSize, BoardSize];
        private IGameManager _gameManager;

        public void SetGridPrefab(GameObject gridPrefab)
        {
            _gridPrefab = gridPrefab;
        }

        public void SetPiecePrefab(GameObject piecePrefab)
        {
            _piecePrefab = piecePrefab;
        }

        public void SetBackgroundPrefab(GameObject backgroundPrefab)
        {
            _gridBackgroundPrefab = backgroundPrefab.GetComponent<SpriteRenderer>();
        }

        public void SetPiece(int x, int y, IGridData gridData)
        {
            if (_gridDatas[x, y].IsCanPut)
            {
                _gridDatas[x, y] = gridData;
            }

            RefreshGrid();
        }

        public IGridData GetPiece(int x, int y) => _gridDatas[x, y];

        public void Initialize()
        {
            Instantiate(_gridBackgroundPrefab);

            for (var x = 0; x < BoardSize; x++)
            {
                for (var y = 0; y < BoardSize; y++)
                {
                    var xLocation = (x * Interval) - EndPoint;
                    var yLocation = (y * Interval) - EndPoint;
                    var gridLocation = new Vector2(xLocation, yLocation);
                    Instantiate(_gridPrefab, gridLocation, Quaternion.identity)
                        .TryGetComponent(out IGridData targetGridData);
                    targetGridData.Initialize(new Point(x, y), gridLocation, GridState.Nopiece);
                    _gridDatas[x, y] = targetGridData;

                    IPiece piece;
                    if (y == 3 && x == 3 || y == 4 && x == 4)
                    {
                        Instantiate(_piecePrefab, gridLocation, Quaternion.identity)
                            .TryGetComponent(out piece);
                        _gridDatas[x, y].AcceptPiece(piece);
                    }

                    if (y == 3 && x == 4 || y == 4 && x == 3)
                    {
                        Instantiate(_piecePrefab, gridLocation, Quaternion.identity)
                            .TryGetComponent(out piece);
                        _gridDatas[x, y].AcceptPiece(piece);
                    }
                }
            }

            _gridDatas[3, 3].ChangeGridState(GridState.Black);
            _gridDatas[4, 4].ChangeGridState(GridState.Black);
            _gridDatas[3, 4].ChangeGridState(GridState.White);
            _gridDatas[4, 3].ChangeGridState(GridState.White);

            RefreshGrid();
        }

        // ReSharper disable once ParameterHidesMember
        public void RefreshGameManager(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void RefreshGrid()
        {
            var blackPiece = 0;
            var whitePiece = 0;
            var canPutGrid = 0;

            for (var x = 0; x < BoardSize; x++)
            {
                for (var y = 0; y < BoardSize; y++)
                {
                    switch (_gridDatas[x, y].GridState)
                    {
                        case GridState.Nopiece:
                            break;
                        case GridState.Black:
                            _gridDatas[x, y].Piece.InitColor(Color.black);
                            blackPiece++;
                            break;
                        case GridState.White:
                            _gridDatas[x, y].Piece.InitColor(Color.white);
                            whitePiece++;
                            break;
                    }

                    SetCanPutGrid(x, y);
                    if (_gridDatas[x, y].GridState == GridState.CanPut)
                    {
                        canPutGrid++;
                    }
                }
            }

            _blackPieceCount.Value = blackPiece;
            _whitePieceCount.Value = whitePiece;
            _canPutGridCount.Value = canPutGrid;
        }

        public void ReceivePieceFromPlayer(IGridData gridData)
        {
            _gridDatas[gridData.Point.X, gridData.Point.Y] = gridData;
        }

        public void FlipPiece(IGridData gridData)
        {
            foreach (var (offsetX, offsetY) in gridData.DirectionOffset)
            {
                FlipPiece(gridData, offsetX, offsetY);
            }
        }

        public Outcome JudgeWinner()
        {
            if (BlackPieceCount.Value > WhitePieceCount.Value)
            {
                return Outcome.Black;
            }

            return WhitePieceCount.Value > BlackPieceCount.Value ? Outcome.White : Outcome.Draw;
        }

        private void FlipPiece(IGridData gridData, int offsetX, int offsetY)
        {
            var nextX = gridData.Point.X + offsetX;
            var nextY = gridData.Point.Y + offsetY;

            if (!IsMyPieceNextGrid(_gridDatas[nextX, nextY].GridState))
            {
                _gridDatas[nextX, nextY].ChangeGridState(gridData.GridState);
                FlipPiece(_gridDatas[nextX, nextY], offsetX, offsetY);
            }
        }

        private void SetCanPutGrid(int currentX, int currentY)
        {
            if (_gridDatas[currentX, currentY].GridState == GridState.CanPut)
            {
                _gridDatas[currentX, currentY].ChangeGridState(GridState.Nopiece);
                _gridDatas[currentX, currentY].DirectionOffset.Clear();
            }

            if (_gridDatas[currentX, currentY].GridState != GridState.Nopiece)
            {
                return;
            }

            try
            {
                for (var offsetX = -1; offsetX < 2; offsetX++)
                {
                    for (var offsetY = -1; offsetY < 2; offsetY++)
                    {
                        var nextX = currentX + offsetX;
                        var nextY = currentY + offsetY;
                        if (IsOutOfGrid(nextX, nextY))
                        {
                            continue;
                        }

                        var nextGridState = _gridDatas[nextX, nextY].GridState;
                        if (IsNoPieceNextGrid(nextGridState) || IsMyPieceNextGrid(nextGridState))
                        {
                            continue;
                        }

                        if (!CanPutGrid(nextX, nextY, offsetX, offsetY)) continue;

                        _gridDatas[currentX, currentY].ChangeGridState(GridState.CanPut);
                        _gridDatas[currentX, currentY].DirectionOffset.Add(new Tuple<int, int>(offsetX, offsetY));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        private bool CanPutGrid(int nextX, int nextY, int offsetX, int offsetY)
        {
            if (IsOutOfGrid(nextX, nextY))
            {
                return false;
            }

            var nextGridState = _gridDatas[nextX, nextY].GridState;
            if (IsNoPieceNextGrid(nextGridState))
            {
                return false;
            }

            if (IsMyPieceNextGrid(nextGridState))
            {
                return true;
            }

            return CanPutGrid(nextX + offsetX, nextY + offsetY, offsetX, offsetY);
        }

        private static bool IsOutOfGrid(int currentX, int currentY)
        {
            return currentX <= -1 || currentX >= 8 || currentY <= -1 || currentY >= 8;
        }

        private static bool IsNoPieceNextGrid(GridState nextGridState)
        {
            return nextGridState == GridState.CanPut || nextGridState == GridState.Nopiece;
        }

        private bool IsMyPieceNextGrid(GridState gridState)
        {
            var gameState = _gameManager.GetGameState();
            return gameState == GameState.BlackTurn && gridState == GridState.Black ||
                   gameState == GameState.WhiteTurn && gridState == GridState.White;
        }
    }
}