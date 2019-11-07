using System;
using System.Linq;
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

		public static int[,] OuterCorner = new int[,]{
			{  0,-32,-32},
			{  8, 8,-32},
			{  8, 8, 0},
		};

		public static int[,] InnerCorner = new int[,]{
			{  4, 4,-8},
			{  4, 4, 4},
			{  4, 4, 4},
		};

		public static int[,] NeighborCount = new int[,]{
			{ 1, 1, 1},
			{ 1, 0, 1},
			{ 1, 1, 1},
		};

		public static int[,] Identity = new int[,]{
			{0,0,0},
			{0,1,0},
			{0,0,0}
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

		/// <summary>
		/// Matrix multiply two int[]
		/// </summary>
		/// <param name="sample"></param>
		/// <param name="filter"></param>
		/// <returns>Matrix of multiplied values</returns>
		public static int[] MultiplyFilter(int[] sample, int[] filter)
		{
			if(sample.Length != filter.Length)
			{
				throw new ArgumentException("sample and filter must be the same size.");
			}
			var outArray = new int[filter.Length];
			for(int i = 0; i < filter.Length; i++)
			{
				outArray[i] = sample[i] * filter[i];
			}
			return outArray;
		}

		public static string ToFilterString(int[,] filter)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < filter.GetLength(0); i++)
			{
				for (int j = 0; j < filter.GetLength(1); j++)
				{
					sb.Append($"{filter[i, j]:##}");
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}
	}

	public static class GridExtentsions
	{
		public static void UpDate(this int[,] grid, Func<int, int> action)
		{
			if (grid.Rank != 2)
			{
				throw new NotImplementedException("Only valid on 2D arrays.");
			}

			for (int i = 0; i < grid.GetLength(0); i++)
			{
				for (int j = 0; j < grid.GetLength(1); j++)
				{
					grid[i, j] = action(grid[i, j]);
				}
			}
		}


		/// <summary>
		/// Total the array elements
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static int Total(this int[] array)
		{
			var total = 0;
			for (int i = 0; i < array.Length; i++)
			{
				total += array[i];
			}
			return total;
		}

		/// <summary>
		/// Total the array elements
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static int Total(this int[,] array)
		{
			var total = 0;
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					total += array[i,j];
				}
			}
			return total;
		}

		public static int[] Flatten(this int[,] sample)
		{
			var width = sample.GetLength(0);
			var height = sample.GetLength(1);
			int[] flat = new int[ width * height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					flat[i + (j * width)] = sample[i,j];
				}
			}
			return flat;
		}

		public static string ToRowString(this int[,] sample, bool asMap = false)
		{
			var rows = sample.GetLength(1);
			var flat = sample.Flatten();
			return flat.ToRowString(rows, asMap);
		}

		/// <summary>
		/// Returns an array as a grid of numRows
		/// </summary>
		/// <param name="sample"></param>
		/// <param name="numRows"></param>
		/// <returns>String representation</returns>
		public static string ToRowString(this int[] sample, int numRows, bool asMap = false)
		{
			var sb = new StringBuilder();
			var perRow = sample.Length / numRows;
			for(int i = 0; i < sample.Length; i++)
			{
				var output = sample[i].ToString();
				if(asMap)
				{
					output = sample[i] == 0 ? "." : "#";
				}
				
				sb.Append(output);
				if(i < sample.Length -1 && ! asMap)
				{
					sb.Append(", ");
				}
				if((i + 1) % perRow == 0)
				{
					sb.AppendLine();
				}
			}
			sb.AppendLine();
			return sb.ToString();
		}
	}
}