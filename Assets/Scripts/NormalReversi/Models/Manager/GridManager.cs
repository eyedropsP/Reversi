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
		[SerializeField] private GameObject gridPrefab;
		[SerializeField] private GameObject gridBackgroundPrefab;
		[SerializeField] private GameObject piecePrefab;
		
		private IntReactiveProperty blackPieceCount = new IntReactiveProperty(0);
		private IntReactiveProperty whitePieceCount = new IntReactiveProperty(0);
		private IntReactiveProperty canPutGridCount = new IntReactiveProperty(0);
		public IReadOnlyReactiveProperty<int> BlackPieceCount => blackPieceCount;
		public IReadOnlyReactiveProperty<int> WhitePieceCount => whitePieceCount;
		public IReadOnlyReactiveProperty<int> CanPutGridCount => canPutGridCount; 
		public const int GridCount = 64;
		
		private const int boardSize = 8;
		private const float endPoint = 3.85f;
		private const float interval = 1.1f;
		private IGridData[,] gridDatas = new IGridData[boardSize,boardSize];
		private IGameManager gameManager;
		
		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			Instantiate(gridBackgroundPrefab);
			for(var x = 0; x < boardSize; x++)
			{
				for (var y = 0; y < boardSize; y++)
				{
					var xLocation = (x * interval) - endPoint;
					var yLocation = (y * interval) - endPoint;
					var gridLocation = new Vector2(xLocation, yLocation);
					Instantiate(gridPrefab, gridLocation, Quaternion.identity)
						.TryGetComponent(out IGridData targetGridData);
					targetGridData.Init(new Point(x, y), gridLocation, GridState.NOPIECE);
					gridDatas[x, y] = targetGridData;

					IPiece piece = null;
					if (y == 3 && x == 3 || y == 4 && x == 4)
					{
						Instantiate(piecePrefab, gridLocation, Quaternion.identity)
							.TryGetComponent(out piece);
						gridDatas[x, y].AcceptPiece(piece);
					}

					if (y == 3 && x == 4 || y == 4 && x == 3)
					{
						Instantiate(piecePrefab, gridLocation, Quaternion.identity)
							.TryGetComponent(out piece);
						gridDatas[x, y].AcceptPiece(piece);
					}
				}
			}
			
			gridDatas[3,3].ChangeGridState(GridState.BLACK);
			gridDatas[4,4].ChangeGridState(GridState.BLACK);
			gridDatas[3,4].ChangeGridState(GridState.WHITE);
			gridDatas[4,3].ChangeGridState(GridState.WHITE);
			RefreshGrid();
		}

		// ReSharper disable once ParameterHidesMember
		public void RefreshGameManager(IGameManager gameManager)
		{
			this.gameManager = gameManager;
		}
		
		public void RefreshGrid()
		{
			var blackPiece = 0;
			var whitePiece = 0;
			var canPutGrid = 0;

			for (var x = 0; x < boardSize; x++)
			{
				for (var y = 0; y < boardSize; y++)
				{
					switch (gridDatas[x, y].GridState)
					{
						case GridState.NOPIECE:
							break;
						case GridState.BLACK:
							gridDatas[x, y].Piece.InitColor(Color.black);
							blackPiece++;
							break;
						case GridState.WHITE:
							gridDatas[x, y].Piece.InitColor(Color.white);
							whitePiece++;
							break;
					}
					SetCanPutGrid(x,y);
					if (gridDatas[x, y].GridState == GridState.CanPut)
					{
						canPutGrid++;
					}
				}
			}

			blackPieceCount.Value = blackPiece;
			whitePieceCount.Value = whitePiece;
			canPutGridCount.Value = canPutGrid;
		}
		
		public void ReceivePieceFromPlayer(IGridData gridData)
		{
			gridDatas[gridData.Point.X, gridData.Point.Y] = gridData;
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
				return Outcome.BLACK;
			}

			if (WhitePieceCount.Value > BlackPieceCount.Value)
			{
				return Outcome.WHITE;
			}

			return Outcome.Draw;
		}

		private void FlipPiece(IGridData gridData, int offsetX, int offsetY)
		{
			var nextX = gridData.Point.X + offsetX;
			var nextY = gridData.Point.Y + offsetY;

			if (!IsMyPieceNextGrid(gridDatas[nextX, nextY].GridState))
			{
				gridDatas[nextX, nextY].ChangeGridState(gridData.GridState);
				FlipPiece(gridDatas[nextX, nextY], offsetX, offsetY);
			}
		}

		private void SetCanPutGrid(int currentX, int currentY)
		{
			if (gridDatas[currentX, currentY].GridState == GridState.CanPut)
			{
				gridDatas[currentX, currentY].ChangeGridState(GridState.NOPIECE);
				gridDatas[currentX, currentY].DirectionOffset.Clear();
			}
			
			if (gridDatas[currentX, currentY].GridState != GridState.NOPIECE)
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
					
						var nextGridState = gridDatas[nextX, nextY].GridState;
						if (IsNoPieceNextGrid(nextGridState) || IsMyPieceNextGrid(nextGridState))
						{
							continue;
						}

						if (!CanPutGrid(nextX, nextY, offsetX, offsetY)) continue;
						
						gridDatas[currentX, currentY].ChangeGridState(GridState.CanPut);
						gridDatas[currentX, currentY].DirectionOffset.Add(new Tuple<int, int>(offsetX, offsetY));
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
			
			var nextGridState = gridDatas[nextX, nextY].GridState;
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
			return nextGridState == GridState.CanPut || nextGridState == GridState.NOPIECE;
		}

		private bool IsMyPieceNextGrid(GridState gridState)
		{
			var gameState = gameManager.GetGameState();
			return gameState == GameState.BLACKTURN && gridState == GridState.BLACK ||
			       gameState == GameState.WHITETURN && gridState == GridState.WHITE;
		}
	}
}