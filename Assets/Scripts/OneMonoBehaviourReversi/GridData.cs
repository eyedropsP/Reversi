using OneMonoBehaviourReversi.Enums;
using OneMonoBehaviourReversi.Interfaces;

namespace OneMonoBehaviourReversi
{
	public class GridData : IGridData
	{
		private GridState _gridState { get; set; }

		public Point Point { get; }

		public GridData(int x, int y, GridState gridState)
		{
			Point = new Point(x, y);
			this._gridState = gridState;
		}

		public void ChangeGridState(GridState gridState)
		{
			this._gridState = gridState;
		}
	}
}