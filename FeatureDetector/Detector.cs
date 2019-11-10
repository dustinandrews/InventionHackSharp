using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace FeatureDetector
{

	public class MapFeatureDetector
	{
		int[,] _mapArray;
		int _xSize, _ySize;
		public MapFeatureDetector(int[,] map)
		{
			_mapArray = map;
			_xSize = map.GetLength(0);
			_ySize = map.GetLength(1);
		}

		public int[,] FindWalls()
		{
			var neighbors = ConvolveFilter(_mapArray, FeatureFilters.NeighborCount);
			var outputArray = new int[_mapArray.GetLength(0),_mapArray.GetLength(1)];
			for (int i = 0; i < neighbors.GetLength(0); i++)
			{
				for (int j = 0; j < neighbors.GetLength(1); j++)
				{
					if (neighbors[i, j] < 8 || _mapArray[i, j] == 1)
					{
						outputArray[i, j] = 1;
					}
				}
			}
			return outputArray;
		}

		public int[,] FindDoorways()
		{
			int[,] outputArray = GetDoorCandidates();

			var neighbors = ConvolveFilter(_mapArray, FeatureFilters.NeighborCount);
			// Cull any spaces without at least 4 solid neighbors and in crowded areas
			// This removes candidates technically in rooms or L bends in corridors
			var density = GetDensityMap();
			for (int i = 0; i < density.GetLength(0); i++)
			{
				for (int j = 0; j < density.GetLength(1); j++)
				{
					if (neighbors[i, j] < 4 || density[i, j] > 35)
					{
						outputArray[i, j] = 0;
					}
				}
			}
			return outputArray;
		}

		private int[,] GetDoorCandidates()
		{
			var list = new List<int[,]>();
			var filter = FeatureFilters.Doorway;
			var outputArray = new int[_xSize, _ySize];

			for (int i = 0; i < 4; i++)
			{
				list.Add(filter);
				filter = FeatureFilters.Rotate90CCW(filter);
			}

			// Mark 1 = up, 2 = right, 3 = down, 4 = left
			var convolutions = ConvolveFilters(_mapArray, list);
			for (int i = 0; i < convolutions.Count(); i++)
			{
				outputArray = Add2DArrays(outputArray, FilterArray(convolutions[i], 8, i + 1));
				outputArray = Add2DArrays(outputArray, FilterArray(convolutions[i], 6, i + 1));
			}

			return outputArray;
		}

		/// <summary>
		/// Given an array with cells of the room set to one number, finds a cell in that list
		/// that is an approximate center of the room. For convex shapes this may not be ideal.
		/// </summary>
		/// <param name="regions"></param>
		/// <param name="roomNum"></param>
		/// <returns></returns>
		public static DetectorPoint GetApproximateRoomCenter(int[,] array, int roomNum, int[,] roomDensity = null)
		{
			if (!array.UniqueValues().Contains(roomNum))
			{
				throw new ArgumentException($"Room {roomNum} not on the map.", nameof(roomNum));
			}

			var a = (int[,])array.Clone();
			if(roomDensity == null)
			{
				roomDensity = GetRoomDensityMap(a);
			}
			a.UpDate(i => (i == roomNum) ? 1 : 0);
			roomDensity = roomDensity.Multiply(a);

			var point = MaxPoint(roomDensity);

			return point;
		}

		private static int[,] GetRoomDensityMap(int[,] a)
		{
			int[,] roomDensity = (int[,]) a.Clone();
			var mask = (int[,]) a.Clone();
			mask.UpDate(i => (i == 0) ? 0 : 1);
			for (int i = 0; i < 3; i++)
			{
				roomDensity = ConvolveFilter(roomDensity, FeatureFilters.NeighborCount);
				roomDensity = roomDensity.Multiply(mask); // Cull non-room cells
			}
			return roomDensity;
		}

		public static DetectorPoint[] GetRoomCenters(int[,] array)
		{
			var roomDensity = GetRoomDensityMap(array);
			var maxRoom = array.Max();
			var points = new List<DetectorPoint>();
			for (int i = array.Min(); i <= maxRoom; i++)
			{
				var point = GetApproximateRoomCenter(array, i, roomDensity);
				points.Add(point);
			}
			return points.ToArray();
		}

		static DetectorPoint MaxPoint(int[,] array)
		{
			GetWidthAndHeight(array, out var width, out var height);
			var point = new DetectorPoint(0,0);
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if(array[x,y] > array[point.X, point.Y])
					{
						point = new DetectorPoint(x,y);
					}
				}
			}
			return point;
		}
		static void GetWidthAndHeight(int[,] array, out int width, out int height)
		{
			width = array.GetLength(0);
			height = array.GetLength(1);
		}

		static DetectorPoint[] PathToLargest(int[,] array, int x, int y)
		{
			var points = new List<DetectorPoint>();
			var next = GetBiggestNeighbor(array, x, y);
			while(array[next.X, next.Y] > array[x, y])
			{
				points.Add(next);
				x = next.X;
				y = next.Y;
			}
			return points.ToArray();
		}

		static DetectorPoint GetBiggestNeighbor(int[,] array, int x, int y)
		{
			var points = GetNeighbors(array, x, y);
			var current = int.MinValue;
			DetectorPoint output = new DetectorPoint(-1,-1);
			foreach(var p in points)
			{
				if(array[p.X, p.Y] > current)
				{
					output = new DetectorPoint(p.X, p.Y);
				}
			}

			if(output.X == -1 || output.Y == -1)
			{
				throw new ArgumentException("Well that was unexpected. Egregious internal error.");
			}
			return output;
		}

		static DetectorPoint[] GetNeighbors(int[,] array, int x, int y)
		{
			GetWidthAndHeight(array, out var width,  out var height);
			var points = new List<DetectorPoint>();
			for (int xi = -1; xi < 2; xi++)
			{
				for (int yi = -1; yi < 2; yi++)
				{
					var px = x + xi;
					var py = y + yi;
					if (IsInBounds(width, height, px, py))
					{
						points.Add(new DetectorPoint(px, py));
					}
				}
			}
			return points.ToArray();
		}

		private static bool IsInBounds(int width, int height, int x, int y)
		{
			return (x < 0 || x >= width || y < 0 || y >= height);
		}

		public class DetectorPoint
		{
			public int X;
			public int Y;
			internal DetectorPoint(int x, int y)
			{
				X = x; Y = y;
			}
		}

		int[,] Add2DArrays(int[,] one, int[,] two)
		{
			var width = one.GetLength(0);
			var height = one.GetLength(1);
			if(two.GetLength(0) != width || two.GetLength(1) != height)
			{
				throw new ArgumentException("Arrays must be the same size.");
			}
			var outArray = new int[width, height];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					outArray[x,y] = one[x,y] + two[x,y];
				}
			}
			return outArray;
		}

		public int[,] FilterArray(int[,] array, int match, int filterValue = 1)
		{
			var outArray = new int[array.GetLength(0), array.GetLength(1)];
			for(int x = 0; x < _xSize; x++)
			{
				for(int y = 0; y < _ySize; y++)
				{
					if(array[x,y] != match)
					{
						outArray[x,y] = 0;
					}
					else
					{
						outArray[x,y] = filterValue;
					}
				}
			}
			//return  outArray;
			return outArray;
		}

		public static int[,] ConvolveFilter(int[,] inputArray, int [,] filter, int padding = 1)
		{
			var width = inputArray.GetLength(0);
			var height = inputArray.GetLength(1);
			var fWidth = filter.GetLength(0);
			if(fWidth != filter.GetLength(1) || fWidth % 2 == 0)
			{
				throw new ArgumentException("Filter must be square and an odd length.");
			}

			var outArray = new int[width,height];
			var kernelSize = (filter.GetLength(0) - 1) / 2;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var value = 0;
					for (int i = 0; i < fWidth; i++)
					{
						for (int j = 0; j < fWidth; j++)
						{
							int arrayVal;
							var xVal = x - kernelSize + i;
							var yVal = y - kernelSize + j; 
							if( IsInBounds(width, height, xVal, yVal))
							{
								arrayVal = padding;
							}
							else
							{
								arrayVal = inputArray[xVal, yVal];
							}
							value += arrayVal * filter[i,j];
						}
						
					}
					outArray[x,y] = value;
				}
			}

			return outArray;
		}

		/// <summary>
		/// By convolving the number of neighbors 2 times each cell has a number indicating how many neighbors
		/// it has in the 64 nearest blocks.
		/// </summary>
		/// <returns></returns>
		int[,] GetDensityMap()
		{
			var conv = ConvolveFilter(_mapArray, FeatureFilters.NeighborCount);
			var density = ConvolveFilter(_mapArray, FeatureFilters.NeighborCount);
			return density;
		}

		public static List<int[,]> ConvolveFilters(int[,] array, List<int[,]> filters)
		{
			var outputArrays = new List<int[,]>();
			for(int i = 0; i < filters.Count; i++)
			{
				var conv = ConvolveFilter(array, filters[i]);
				outputArrays.Add(conv);
			}

			return outputArrays;
		}

		public string ToMapString(int[,] map)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("   ");
			for(int w = 0; w < map.GetLength(1); w++)
			{
				sb.Append(w % 10);
			}
			sb.AppendLine();
			for(int i = 0; i < map.GetLength(0); i++)
			{
				sb.Append($"{i:D2} ");
				for(int j = 0; j < map.GetLength(1); j++)
				{
					if(map[i,j] > 0)
					{
						sb.Append(map[i,j]);
					}
					else
					{
						sb.Append(_mapArray[i,j] == 1 ? "#": "." );
					}
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}

		/// <summary>
		/// Takes and array of 1 and 0's. Fills in each region of zeros with numbers starting at 2.
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static int[,] GetRegions(int[,] array)
		{
			var width = array.GetLength(0);
			var height = array.GetLength(1);
			var returnArray = (int[,]) array.Clone();

			var copy = (int[,]) array.Clone();
			int fillNum = 2;
			for (int x1 = 0; x1 < width; x1++)
			{
				for (int y1 = 0; y1 < height; y1++)
				{
					if (copy[x1,y1] == 0)
					{
						AdamMilFill(copy, x1, y1);
						copy = FillRegion(returnArray, copy, fillNum);
						returnArray = (int[,]) copy.Clone();
						fillNum++;
					}
				}
			}

			return returnArray;
		}

		static int[,] FillRegion(int[,] filled, int[,] compare, int fillNum)
		{
			var width = filled.GetLength(0);
			var height = filled.GetLength(1);
		
			var copy = (int[,]) filled.Clone();
			for (int x1 = 0; x1 < width; x1++)
			{
				for (int y1 = 0; y1 < height; y1++)
				{
					if (copy[x1,y1] == 0 && compare[x1,y1] == 1)
					{
						copy[x1,y1] = fillNum;
					}
				}
			}
			return copy;
		}

		public static void AdamMilFill(int[,] array, int x, int y)
		{
			var width = array.GetLength(0);
			var height = array.GetLength(1);
			var boolArray = new bool[height, width];
			for (int x1 = 0; x1 < width; x1++)
			{
				for (int y1 = 0; y1 < height; y1++)
				{
					boolArray[y1,x1] = array[x1,y1] == 1;
				}
			}

			AdamMilFill(boolArray, x, y);

			for (int x1 = 0; x1 < width; x1++)
			{
				for (int y1 = 0; y1 < height; y1++)
				{
					array[x1,y1] = boolArray[y1,x1] ? 1 : 0;
				}
			}
		}

		/// <summary>
		/// http://www.adammil.net/blog/v126_A_More_Efficient_Flood_Fill.html
		/// </summary>
		/// <param name="array"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void AdamMilFill(bool[,] array, int x, int y)
		{
			if (!array[y, x]) _AdamMilFill(array, x, y, array.GetLength(1), array.GetLength(0));
		}

		static void _AdamMilFill(bool[,] array, int x, int y, int width, int height)
		{
			// at this point, we know array[y,x] is clear, and we want to move as far as possible to the upper-left. moving
			// up is much more important than moving left, so we could try to make this smarter by sometimes moving to
			// the right if doing so would allow us to move further up, but it doesn't seem worth the complexity
			while (true)
			{
				int ox = x, oy = y;
				while (y != 0 && !array[y - 1, x]) y--;
				while (x != 0 && !array[y, x - 1]) x--;
				if (x == ox && y == oy) break;
			}
			AdamMilFillCore(array, x, y, width, height);
		}

		static void AdamMilFillCore(bool[,] array, int x, int y, int width, int height)
		{
			// at this point, we know that array[y,x] is clear, and array[y-1,x] and array[y,x-1] are set.
			// we'll begin scanning down and to the right, attempting to fill an entire rectangular block
			int lastRowLength = 0; // the number of cells that were clear in the last row we scanned
			do
			{
				int rowLength = 0, sx = x; // keep track of how long this row is. sx is the starting x for the main scan below
										   // now we want to handle a case like |***|, where we fill 3 cells in the first row and then after we move to
										   // the second row we find the first  | **| cell is filled, ending our rectangular scan. rather than handling
										   // this via the recursion below, we'll increase the starting value of 'x' and reduce the last row length to
										   // match. then we'll continue trying to set the narrower rectangular block
				if (lastRowLength != 0 && array[y, x]) // if this is not the first row and the leftmost cell is filled...
				{
					do
					{
						if (--lastRowLength == 0) return; // shorten the row. if it's full, we're done
					} while (array[y, ++x]); // otherwise, update the starting point of the main scan to match
					sx = x;
				}
				// we also want to handle the opposite case, | **|, where we begin scanning a 2-wide rectangular block and
				// then find on the next row that it has     |***| gotten wider on the left. again, we could handle this
				// with recursion but we'd prefer to adjust x and lastRowLength instead
				else
				{
					for (; x != 0 && !array[y, x - 1]; rowLength++, lastRowLength++)
					{
						array[y, --x] = true; // to avoid scanning the cells twice, we'll fill them and update rowLength here
											  // if there's something above the new starting point, handle that recursively. this deals with cases
											  // like |* **| when we begin filling from (2,0), move down to (2,1), and then move left to (0,1).
											  // the  |****| main scan assumes the portion of the previous row from x to x+lastRowLength has already
											  // been filled. adjusting x and lastRowLength breaks that assumption in this case, so we must fix it
						if (y != 0 && !array[y - 1, x]) _AdamMilFill(array, x, y - 1, width, height); // use _Fill since there may be more up and left
					}
				}

				// now at this point we can begin to scan the current row in the rectangular block. the span of the previous
				// row from x (inclusive) to x+lastRowLength (exclusive) has already been filled, so we don't need to
				// check it. so scan across to the right in the current row
				for (; sx < width && !array[y, sx]; rowLength++, sx++) array[y, sx] = true;
				// now we've scanned this row. if the block is rectangular, then the previous row has already been scanned,
				// so we don't need to look upwards and we're going to scan the next row in the next iteration so we don't
				// need to look downwards. however, if the block is not rectangular, we may need to look upwards or rightwards
				// for some portion of the row. if this row was shorter than the last row, we may need to look rightwards near
				// the end, as in the case of |*****|, where the first row is 5 cells long and the second row is 3 cells long.
				// we must look to the right  |*** *| of the single cell at the end of the second row, i.e. at (4,1)
				if (rowLength < lastRowLength)
				{
					for (int end = x + lastRowLength; ++sx < end;) // 'end' is the end of the previous row, so scan the current row to
					{                                          // there. any clear cells would have been connected to the previous
						if (!array[y, sx]) AdamMilFillCore(array, sx, y, width, height); // row. the cells up and left must be set so use FillCore
					}
				}
				// alternately, if this row is longer than the previous row, as in the case |*** *| then we must look above
				// the end of the row, i.e at (4,0)                                         |*****|
				else if (rowLength > lastRowLength && y != 0) // if this row is longer and we're not already at the top...
				{
					for (int ux = x + lastRowLength; ++ux < sx;) // sx is the end of the current row
					{
						if (!array[y - 1, ux]) _AdamMilFill(array, ux, y - 1, width, height); // since there may be clear cells up and left, use _Fill
					}
				}
				lastRowLength = rowLength; // record the new row length
			} while (lastRowLength != 0 && ++y < height); // if we get to a full row or to the bottom, we're done
		}
	}
}
