using System.Text;

namespace MegaDungeon
{
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

		

		public static int Total(this int[] array)
		{
			var total = 0;
			for(int i = 0; i < array.Length; i++)
			{
				total += array[i];
			}
			return total;
		}

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
