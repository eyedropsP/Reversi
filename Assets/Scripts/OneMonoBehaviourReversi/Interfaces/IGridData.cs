using OneMonoBehaviourReversi.Enums;

namespace OneMonoBehaviourReversi.Interfaces
{
	public interface IGridData
	{
		Point Point { get; }

		void ChangeGridState(GridState gridState);
	}
}