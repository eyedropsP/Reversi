namespace NormalReversi.Extensions
{
	public static class ArrayExtension
	{
		public static T GetOrDefault<T>(this T[,] array, int xIndex, int yIndex, T defaultValue = default)
		{
			if ((array?.Length ?? 0) == 0)
			{
				return defaultValue;
			}

			if (xIndex < 0 || yIndex < 0)
			{
				return defaultValue;
			}

			if (xIndex >= array.GetLength(0) || yIndex >= array.GetLength(1))
			{
				return defaultValue;
			}

			return array[xIndex, yIndex];
		}
	}
}