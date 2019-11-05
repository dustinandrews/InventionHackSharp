using System;
using System.Text;

namespace FeatureDetector
{
	public static class FeatureFilters
	{
		// 1 == impassable/wall
		public static int[,] Vertical = new int[,]{
			{-1, 4,-1},
			{-1, 4,-1},
			{-1, 4,-1}
		};

		public static int[,] Doorway = new int[,]{
			{ -2,-16,-2},
			{  2,-16, 2},
			{  2,-16, 2},
		};
		
		public static int[,] OuterCorner= new int[,]{
			{  0,-32,-32},
			{  8, 8,-32},
			{  8, 8, 0},
		};

		public static int[,] InnerCorner= new int[,]{
			{  4, 4,-8},
			{  4, 4, 4},
			{  4, 4, 4},
		};

		public static int[,] NeighborCount = new int[,]{
			{ 1, 1, 1},
			{ 1, 0, 1},
			{ 1, 1, 1},
		};

		public static int[,] Rotate180Degrees(int[,] oldMatrix)
		{
			int[,] newMatrix = Rotate90CCW(oldMatrix);
			newMatrix = Rotate90CCW(newMatrix);
			return newMatrix;
		}

		public static int[,] Rotate90CCW(int[,] oldMatrix)
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

		public static string ToFilterString(int[,] filter)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < filter.GetLength(0); i++)
			{
				for (int j = 0; j < filter.GetLength(1); j++)
				{
					sb.Append($"{filter[i,j]:##}");
				}
				sb.AppendLine();
			}
			return sb.ToString();
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
