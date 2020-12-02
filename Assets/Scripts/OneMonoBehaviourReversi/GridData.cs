using OneMonoBehaviourReversi.Enums;
using OneMonoBehaviourReversi.Interfaces;

namespace OneMonoBehaviourReversi
{
	public class GridData : IGridData
	{
		public GridState gridState { get; private set; }

		public Point Point { get; }

		public GridData(int x, int y, GridState gridState)
		{
			Point = new Point(x, y);
			this.gridState = gridState;
		}

		public void ChangeGridState(GridState gridState)
		{
			this.gridState = gridState;
		}
	}
}