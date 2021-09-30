using NormalReversi.Models;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Manager;
using NUnit.Framework;
using UniRx;
using UnityEngine;

namespace Tests
{
    public class GridTests
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly GameObject _gridManagerGameObject = new GameObject();
        private readonly GameObject _gridDataGameObject = new GameObject();
        private readonly GameObject _pieceGameObject = new GameObject();
        private readonly GameObject _backgroundPrefab = new GameObject();
        private readonly IPlayer _player = new Player();
        private IGridManager _gridManager;
        private IGameStateManager _gameStateManager;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _gridManager = _gridManagerGameObject.AddComponent<GridManager>();
            _gridDataGameObject.AddComponent<GridData>();
            _gridDataGameObject.AddComponent<SpriteRenderer>();
            _gridDataGameObject.AddComponent<BoxCollider2D>();
            _pieceGameObject.AddComponent<Piece>();
            _pieceGameObject.AddComponent<SpriteRenderer>();
            _backgroundPrefab.AddComponent<SpriteRenderer>();
        }
        
        [SetUp]
        public void SetUp()
        {
            _gameStateManager = new GameStateManager();
            _gridManager = _gridManagerGameObject.GetComponent<GridManager>();

            var gridDataSpriteRenderer = _gridDataGameObject.GetComponent<SpriteRenderer>();
            _gridDataGameObject.GetComponent<IGridData>().SetSpriteRenderer(gridDataSpriteRenderer);

            var pieceSpriteRenderer = _pieceGameObject.GetComponent<SpriteRenderer>();
            _pieceGameObject.GetComponent<IPiece>().SetSpriteRenderer(pieceSpriteRenderer);
            
            _gridManager.SetGridPrefab(_gridDataGameObject);
            _gridManager.SetPiecePrefab(_pieceGameObject);
            _gridManager.SetBackgroundPrefab(_backgroundPrefab);

            _gridManager.RefreshGameManager(_gameStateManager);
            _gridManager.Initialize();

            _gridManager.CanPutGridCount
                .Where(value => value == 0)
                .Subscribe(value =>
                {
                    _gameStateManager.ChangeGameState();
                    _gridManager.RefreshGameManager(_gameStateManager);
                    _gridManager.RefreshGrid();

                    if (_gridManager.CanPutGridCount.Value == 0)
                    {
                        _gameStateManager.GameSet();
                    }
                }).AddTo(_disposable);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _disposable.Dispose();
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
        public void 反転のテスト()
        {
            ProgressTurn(5,3);

            Assert.That(_gridManager.CanPutGridCount.Value, Is.EqualTo(3));
            Assert.That(_gridManager.BlackPieceCount.Value, Is.EqualTo(4));
            Assert.That(_gridManager.WhitePieceCount.Value, Is.EqualTo(1));
            Assert.That(_gridManager.GetPiece(3, 2).GridState, Is.EqualTo(GridState.CanPut));
            Assert.That(_gridManager.GetPiece(5, 4).GridState, Is.EqualTo(GridState.CanPut));
            Assert.That(_gridManager.GetPiece(5, 2).GridState, Is.EqualTo(GridState.CanPut));
        }

        [Test]
        public void 白最短全滅()
        {
            ProgressTurn(5, 3);
            ProgressTurn(5, 2);
            ProgressTurn(4, 2);
            ProgressTurn(5, 4);
            ProgressTurn(4, 5);
            ProgressTurn(3, 6);
            ProgressTurn(3, 5);
            ProgressTurn(3, 2);
            ProgressTurn(2, 4);
            ProgressTurn(1, 4);
            var outcome = _gridManager.JudgeWinner();

            Assert.That(_gridManager.BlackPieceCount.Value, Is.Zero);
            Assert.That(_gridManager.WhitePieceCount.Value, Is.EqualTo(14));
            Assert.That(_gameStateManager.NowGameState.Value, Is.EqualTo(GameState.GameSet));
            Assert.That(outcome, Is.EqualTo(Outcome.White));
        }

        [Test]
        public void 黒最短全滅()
        {
            ProgressTurn(5, 3);
            ProgressTurn(3, 2);
            ProgressTurn(2, 3);
            ProgressTurn(5, 4);
            ProgressTurn(4, 1);
            ProgressTurn(5, 2);
            ProgressTurn(6, 3);
            ProgressTurn(4, 2);
            ProgressTurn(4, 5);
            var outcome = _gridManager.JudgeWinner();

            Assert.That(_gridManager.BlackPieceCount.Value, Is.EqualTo(13));
            Assert.That(_gridManager.WhitePieceCount.Value, Is.Zero);
            Assert.That(_gameStateManager.NowGameState.Value, Is.EqualTo(GameState.GameSet));
            Assert.That(outcome, Is.EqualTo(Outcome.Black));
        }

        [Test]
        public void 終局11手()
        {
            ProgressTurn(5, 3);
            ProgressTurn(5, 4);
            ProgressTurn(2, 5);
            ProgressTurn(4, 2);
            ProgressTurn(5, 1);
            ProgressTurn(6, 2);
            ProgressTurn(6, 4);
            ProgressTurn(4, 0);
            ProgressTurn(6, 0);
            ProgressTurn(6, 3);
            ProgressTurn(7, 3);
            var outcome = _gridManager.JudgeWinner();
            
            Assert.That(_gridManager.BlackPieceCount.Value, Is.EqualTo(14));
            Assert.That(_gridManager.WhitePieceCount.Value, Is.EqualTo(1));
            Assert.That(_gridManager.CanPutGridCount.Value, Is.Zero);
            Assert.That(_gameStateManager.NowGameState.Value, Is.EqualTo(GameState.GameSet));
            Assert.That(outcome, Is.EqualTo(Outcome.Black));
        }

        [Test]
        public void パス最短9手()
        {
            ProgressTurn(5,3);
            ProgressTurn(5,2);
            ProgressTurn(3,5);
            ProgressTurn(6,3);
            ProgressTurn(7,3);
            ProgressTurn(7,4);
            ProgressTurn(5,1);
            ProgressTurn(7,2);
            
            Assert.That(_gameStateManager.NowGameState.Value, Is.EqualTo(GameState.WhiteTurn));
        }
        
        private void ProgressTurn(int x, int y)
        {
            var gridData = _gridManager.GetPiece(x, y);
            var playerPutGridData = _player.Put(gridData, _gameStateManager);
            _gridManager.ReceivePieceFromPlayer(playerPutGridData);
            _gridManager.FlipPiece(playerPutGridData);
            _gameStateManager.ChangeGameState();
            _gridManager.RefreshGameManager(_gameStateManager);
            _gridManager.RefreshGrid();
        }
    }
}