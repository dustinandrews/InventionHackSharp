namespace FeatureDetector
{
    public static class FeatureFilters
	{
		// 1 == impassable/wall
		public static int[,] Vertical = new int[,]{
			{-1,2,-1},
			{-1,2,-1},
			{-1,2,-1}
		};

		public static int[,] Cross = new int[,]{
			{-1, 2,-1},
			{ 2, 2, 2},
			{-1, 2, -1}
		};

		public static int[,] Doorway = new int[,]{
			{ 2, 2, 2},
			{-1, 2,-1},
			{-1, 2,-1},
		};

		public static int[,] RotateMatrixCounterClockwise(int[,] oldMatrix)
		{
			int[,] newMatrix = new int[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];
			int newColumn, newRow = 0;
			for (int oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
			{
				newColumn = 0;
				for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
				{
					newMatrix[newRow, newColumn] = oldMatrix[oldRow, oldColumn];
					newColumn++;
				}
				newRow++;
			}
			return newMatrix;
		}
	}
}
