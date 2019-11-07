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
							if( xVal < 0 || xVal >= width ||
							    yVal < 0 || yVal >= height)
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
	}
}
