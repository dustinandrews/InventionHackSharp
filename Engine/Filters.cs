using System;
using System.Text;
using RogueSharp;
using static MegaDungeon.EngineConstants;

namespace MegaDungeon
{
	/// <summary>
	/// Filters for matching small segments of the dungeon against to detect features.
	/// A simplified form of convalutional filters.
	/// </summary>
	public static class Filters
	{
		// 1 == impassable
		public static int[] Vertical = new []{
			0,1,0,
			0,0,0,
			0,1,0
		};

		public static int[] Horizontal = new []{
			0,0,0,
			1,0,1,
			0,0,0
		};

		public static int[] Corner_Bottom_Left = new []{
			0,1,0,
			1,1,0,
			0,0,0
		};

		public static int[] Corner_Bottom_Right = new []{
			0,1,0,
			0,1,1,
			0,0,0
		};

		public static int[] Corner_Top_Left = new []{
			0,0,0,
			0,1,1,
			0,1,0
		};

		public static int[] Corner_Top_Right = new []{
			0,0,0,
			1,1,0,
			0,1,0
		};

		public static int[] Corridor_Horizontal = new []{
			1,1,1,
			0,0,0,
			1,1,1
		};

		public static int[] Corridor_Vertical = new []{
			1,0,1,
			1,0,1,
			1,0,1
		};
		public static int[] InnerCornerBottomLeft = new[]{
			1,1,0,
			1,1,1,
			1,1,1
			};

		public static int[] InnerCornerBottomRight = new[]{
			0,1,1,
			1,1,1,
			1,1,1
			};

		public static int[] InnerCornerTopLeft = new[]{
			1,1,1,
			1,1,1,
			1,1,0
			};

		public static int[] InnerCornerTopRight = new[]{
			1,1,1,
			1,1,1,
			0,1,1
			};

		public static int[] DoorWay = new []{
			0,0,0,
			1,0,1,
			1,0,1
		};

		public static int[] DoorWay2 = new []{
			1,0,1,
			1,0,1,
			0,0,0
		};

		public static int[] DoorWay3 = new []{
			0,1,1,
			0,0,0,
			0,1,1
		};

		public static int[] DoorWay4 = new []{
			1,1,0,
			0,0,0,
			1,1,0
		};

		/// <summary>
		/// Create an int[9] array with the cell and it's neighbors with 1 for blocked and 0 for open
		/// </summary>
		/// <param name="cell"></param>
		/// <param name="map"></param>
		/// <returns></returns>
		/// <remarks>Array order top-left,top,top-right,left,center,right,bottomleft,bottom,bottomright
		public static int[] GetCellFilterArray(ICell cell, RogueSharp.IMap map)
		{
			var outArray = new int[COMPASSPOINTS.Length];
			for(int i =0 ; i < COMPASSPOINTS.Length; i++)
			{
				var point = new Point(cell.X, cell.Y) + COMPASSPOINTS[i];
				outArray[i] = 1;

				if(point.X > -1 && point.X < map.Width && point.Y > -1 && point.Y < map.Height)
				{
					if(map.GetCell(point.X, point.Y).IsWalkable)
					{
						outArray[i] = 0;
					}
				}
			}
			return outArray;
		}

		/// <summary>
		/// Total the array elements
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static int Total(this int[] array)
		{
			var total = 0;
			for(int i = 0; i < array.Length; i++)
			{
				total += array[i];
			}
			return total;
		}

		/// <summary>
		/// Compare two int arrays
		/// </summary>
		/// <param name="sample"></param>
		/// <param name="filter"></param>
		/// <returns>True if both arrays total the same</returns>
		public static bool FilterMatch(int[] sample, int[] filter)
		{
			var total = FilterTotal(sample, filter);
			var ftotal = filter.Total();
			return total == ftotal;
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

		/// <summary>
		/// Matrix multiplies two arrays and returns the total of the product array.
		/// </summary>
		/// <param name="sample"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static int FilterTotal(int[] sample, int[] filter)
		{
			var mult = MultiplyFilter(sample, filter);
			return mult.Total();
		}

		/// <summary>
		/// Returns an array as a grid of numRows
		/// </summary>
		/// <param name="sample"></param>
		/// <param name="numRows"></param>
		/// <returns>String representation</returns>
		public static string ToRowString(this int[] sample, int numRows)
		{
			var sb = new StringBuilder();
			var perRow = sample.Length / numRows;
			for(int i = 0; i < sample.Length; i++)
			{
				sb.Append(sample[i]);
				if(i < sample.Length -1)
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
