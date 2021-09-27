using System.Collections.Generic;
using NormalReversi.Models;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Manager;
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
        public void 白の最短全滅()
        {
            
        }

        [Test]
        public void 黒の最短全滅()
        {
            
        }
    }
}