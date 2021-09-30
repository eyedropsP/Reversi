namespace NormalReversi.Models.Interface
{
	public interface IPlayer
	{
		IGridData Put(IGridData gridData, IGameStateManager gameStateManager);
	}
}