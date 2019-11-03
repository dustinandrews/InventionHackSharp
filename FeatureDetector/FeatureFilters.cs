using System;

namespace FeatureDetector
{
	public static class FeatureFilters
	{
		// 1 == impassable/wall
		public static int[,] Vertical = new int[,]{
			{-1,8,-1},
			{-4,16,-4},
			{-1,8,-1}
		};

		public static int[,] Cross = new int[,]{
			{ 8, -1, 8},
			{-1,-32,-1},
			{ 8, -1, 8}
		};

		public static int[,] Doorway = new int[,]{
			{ -2,-16,-2},
			{  2,-16, 2},
			{  2,-16, 2},
		};
		
		public static int[,] OuterCorner= new int[,]{
			{ -4,-4,-4},
			{  2, 2,-4},
			{  2, 2,-4},
		};

		public static int[,] InnerCorner= new int[,]{
			{  1, 1,-8},
			{  1, 1, 1},
			{  1, 1, 1},
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

	public static class GridExtentsions
	{
		public static void UpDate(this int[,] grid, Func<int, int> action )
		{
			if(grid.Rank != 2)
			{
				throw new NotImplementedException("Only valid on 2D arrays.");
			}

			for(int i = 0; i < grid.GetLength(0); i++)
			{
				for(int j = 0; j < grid.GetLength(1); j++)
				{
					grid[i,j]= action(grid[i,j]);
				}
			}
		}
	}
}
