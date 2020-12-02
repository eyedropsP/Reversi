using System.Collections.Generic;
using OneMonoBehaviourReversi.Enums;
using OneMonoBehaviourReversi.Interfaces;

namespace OneMonoBehaviourReversi
{
	public class GridController
	{
		public static readonly int gridSize = 8;
		private List<IGridData> gridDatas;
		private IPiece piece;

		public GridController(IPiece piece)
		{
			this.piece = piece;
			gridDatas = new List<IGridData>();
		}

		private void InitializeGrid(int x, int y, GridState gridState)
		{
			var gridData = new GridData(x, y, gridState);
			gridDatas.Add(gridData);
		}

		private void RefreshGrid()
		{
			
		}
		
		
	}
}