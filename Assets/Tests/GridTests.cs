using NormalReversi.Models;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Manager;
using NormalReversi.Models.Struct;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class GridTests
    {
        private readonly GameObject _gridManagerGameObject = new GameObject();
        private readonly GameObject _gridDataGameObject = new GameObject();
        private readonly GameObject _pieceGameObject = new GameObject();
        private readonly GameObject _backgroundPrefab = new GameObject();
        private IGridManager _gridManager;
        private IGameManager _gameManager;

        [SetUp]
        public void SetUp()
        {
            _gridManager = _gridManagerGameObject.AddComponent<GridManager>().GetComponent<GridManager>();
            _gameManager = new GameManager();

            _gridDataGameObject.AddComponent<GridData>();
            _gridDataGameObject.AddComponent<SpriteRenderer>();
            var gridDataSpriteRenderer = _gridDataGameObject.GetComponent<SpriteRenderer>();
            _gridDataGameObject.GetComponent<IGridData>().SetSpriteRenderer(gridDataSpriteRenderer);
            _gridDataGameObject.AddComponent<BoxCollider2D>();

            _pieceGameObject.AddComponent<Piece>();
            _pieceGameObject.AddComponent<SpriteRenderer>();
            var pieceSpriteRenderer = _pieceGameObject.GetComponent<SpriteRenderer>();
            _pieceGameObject.GetComponent<IPiece>().SetSpriteRenderer(pieceSpriteRenderer);

            _backgroundPrefab.AddComponent<SpriteRenderer>();

            _gridManager.SetGridPrefab(_gridDataGameObject);
            _gridManager.SetPiecePrefab(_pieceGameObject);
            _gridManager.SetBackgroundPrefab(_backgroundPrefab);

            _gridManager.RefreshGameManager(_gameManager);
            _gridManager.Initialize();
        }

        [Test]
        public void 初期状態は真ん中4マスに駒が置かれている()
        {
            var gridBlackPiece1 = _gridManager.GetPiece(3, 3);
            var gridBlackPiece2 = _gridManager.GetPiece(4, 4);
            var gridWhitePiece1 = _gridManager.GetPiece(3, 4);
            var gridWhitePiece2 = _gridManager.GetPiece(4, 3);

            Assert.That(gridBlackPiece1.GridState, Is.EqualTo(GridState.Black));
            Assert.That(gridBlackPiece2.GridState, Is.EqualTo(GridState.Black));
            Assert.That(gridWhitePiece1.GridState, Is.EqualTo(GridState.White));
            Assert.That(gridWhitePiece2.GridState, Is.EqualTo(GridState.White));
        }

        [Test]
        public void 初期状態で先手が置けるマスのテスト()
        {
            Assert.That(_gridManager.GetPiece(4, 2).GridState, Is.EqualTo(GridState.CanPut));
            Assert.That(_gridManager.GetPiece(5, 3).GridState, Is.EqualTo(GridState.CanPut));
            Assert.That(_gridManager.GetPiece(2, 4).GridState, Is.EqualTo(GridState.CanPut));
            Assert.That(_gridManager.GetPiece(3, 5).GridState, Is.EqualTo(GridState.CanPut));
        }

        [Test]
        public void 黒を53に置いた時の白が置けるマスのテスト()
        {
            IPlayer player = new Player();
            var gridData = _gridManager.GetPiece(5, 3);
            var playerPutGridData = player.Put(gridData, _gameManager);
            _gridManager.ReceivePieceFromPlayer(playerPutGridData);
            _gridManager.FlipPiece(playerPutGridData);
            _gameManager.ChangeGameState();
            _gridManager.RefreshGameManager(_gameManager);
            _gridManager.RefreshGrid();

            Assert.That(_gridManager.GetPiece(3, 2).GridState, Is.EqualTo(GridState.CanPut));
            Assert.That(_gridManager.GetPiece(5, 4).GridState, Is.EqualTo(GridState.CanPut));
            Assert.That(_gridManager.GetPiece(5, 2).GridState, Is.EqualTo(GridState.CanPut));
        }

        [Test]
        public void 反転のテスト()
        {
            IPlayer player = new Player();
            var gridData = _gridDataGameObject.GetComponent<IGridData>();
            gridData.Initialize(new Point(5, 3), Vector2.zero, GridState.Black);
            var playerPutGridData = player.Put(gridData, _gameManager);
            _gridManager.ReceivePieceFromPlayer(playerPutGridData);
            _gridManager.FlipPiece(playerPutGridData);

            _gameManager.ChangeGameState();

            _gridManager.RefreshGameManager(_gameManager);
            _gridManager.RefreshGrid();
        }

        [Test]
        public void 白最短全滅()
        {
            var player = new Player();
            ProgressTurn(player, new Point(5, 3), GridState.Black);
            ProgressTurn(player, new Point(5, 2), GridState.White);
            ProgressTurn(player, new Point(4, 2), GridState.Black);
            ProgressTurn(player, new Point(5, 4), GridState.White);
            ProgressTurn(player, new Point(4, 5), GridState.Black);
            ProgressTurn(player, new Point(3, 6), GridState.White);
            ProgressTurn(player, new Point(3, 5), GridState.Black);
            ProgressTurn(player, new Point(3, 2), GridState.Black);
            ProgressTurn(player, new Point(2, 4), GridState.White);
            ProgressTurn(player, new Point(1, 4), GridState.Black);
            Assert.That(_gridManager.BlackPieceCount.Value, Is.Zero);
            Assert.That(_gridManager.WhitePieceCount.Value, Is.EqualTo(14));
        }

        [Test]
        public void 黒最短全滅()
        {
            var player = new Player();
            ProgressTurn(player, new Point(5, 3), GridState.Black);
            ProgressTurn(player, new Point(3, 2), GridState.White);
            ProgressTurn(player, new Point(2, 3), GridState.Black);
            ProgressTurn(player, new Point(5, 4), GridState.White);
            ProgressTurn(player, new Point(4, 1), GridState.Black);
            ProgressTurn(player, new Point(5, 2), GridState.White);
            ProgressTurn(player, new Point(6, 3), GridState.Black);
            ProgressTurn(player, new Point(4, 2), GridState.Black);
            ProgressTurn(player, new Point(4, 5), GridState.White);
            Assert.That(_gridManager.BlackPieceCount.Value, Is.EqualTo(13));
            Assert.That(_gridManager.WhitePieceCount.Value, Is.Zero);
        }

        private void ProgressTurn(IPlayer player, Point point, GridState state)
        {
            var gridData = _gridManager.GetPiece(point.X, point.Y);
            var playerPutGridData = player.Put(gridData, _gameManager);
            _gridManager.ReceivePieceFromPlayer(playerPutGridData);
            _gridManager.FlipPiece(playerPutGridData);
            _gameManager.ChangeGameState();
            _gridManager.RefreshGameManager(_gameManager);
            _gridManager.RefreshGrid();
        }
    }
}