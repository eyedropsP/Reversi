using System.Collections.Generic;
using OneMonoBehaviourReversi.Enums;
using OneMonoBehaviourReversi.Interfaces;

namespace OneMonoBehaviourReversi
{
	public class GridController
	{
		public static readonly int GridSize = 8;
		private readonly List<IGridData> _gridDatas;
		private IPiece _piece;

		public GridController(IPiece piece)
		{
			_piece = piece;
			_gridDatas = new List<IGridData>();
		}

		private void InitializeGrid(int x, int y, GridState gridState)
		{
			var gridData = new GridData(x, y, gridState);
			_gridDatas.Add(gridData);
		}

		private void RefreshGrid()
		{
			
		}
		
		
	}
}