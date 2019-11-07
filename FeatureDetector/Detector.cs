using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using NumSharp;

namespace FeatureDetector
{
	public class MapFeatureDetector
	{
		NDArray _mapArray;
		NDArray _paddedArray;
		int _xSize, _ySize;
		public MapFeatureDetector(int[,] map)
		{
			CreateNdArrays(map);
		}

		public int[,] FindWalls()
		{
			var neighbors = (int[,])ConvolveFilter(FeatureFilters.NeighborCount, 1).ToMuliDimArray<int>();
			var outputArray = new int[_mapArray.shape[0],_mapArray.shape[1]];
			for (int i = 0; i < neighbors.GetLength(0); i++)
			{
				for (int j = 0; j < neighbors.GetLength(1); j++)
				{
					if (neighbors[i, j] < 8 || _mapArray[i, j].Data<int>()[0] == 1)
					{
						outputArray[i, j] = 1;
					}
				}
			}
			return outputArray;
		}

		public int[,] FindDoorways()
		{
			NDArray outputArray = GetDoorCandidates();

			var neighbors = (int[,])ConvolveFilter(FeatureFilters.NeighborCount, 1).ToMuliDimArray<int>();
			// Cull any spaces without at least 4 solid neighbors and in crowded areas
			// This removes candidates technically in rooms or L bends in corridors
			var density = (int[,])GetDensityMap().ToMuliDimArray<int>();
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
			// 23,43
			return (int[,])outputArray.ToMuliDimArray<int>();
		}

		public NDArray FilterArray(NDArray array, int match, int filterValue = 1)
		{
			var outArray = np.zeros_like(array);
			for(int x = 0; x < _xSize; x++)
			{
				for(int y = 0; y < _ySize; y++)
				{
					if(array[x,y].Data<int>()[0] != match)
					{
						outArray[x,y] = 0;
					}
					else
					{
						outArray[x,y] = filterValue;
					}
				}
			}
			//return (int[,]) outArray.ToMuliDimArray<int>();
			return outArray;
		}
		

		public NDArray ConvolveFilter(int[,] filter, int padding)
		{
			return ConvolveFilter(filter, _paddedArray, padding);
		}

		public static int[,] ConvolveFilter(int[,] inputArray, int [,] filter)
		{
			var padding = filter.GetLength(0) - 2;
			var padded = GetPaddedArray(inputArray, padding);
			var conv = ConvolveFilter(filter, padded, padding);
			return (int[,]) conv.ToMuliDimArray<int>();

		}
		public static NDArray ConvolveFilter(int[,] filter, NDArray paddedArray, int padding)
		{
			var list = new List<int[,]>();
			list.Add(filter);
			return ConvolveFilters(list, paddedArray, padding)[0];
		}

		public static NDArray[] ConvolveFilters(List<int[,]> filters, NDArray paddedArray, int padding)
		{
			var ndList = new List<NDArray>();
			foreach(var f in filters)
			{
				ndList.Add(np.array(f));
			}

			return ConvolveFilters(ndList, paddedArray, padding);
		}

		private NDArray GetDoorCandidates()
		{
			var list = new List<int[,]>();
			var filter = FeatureFilters.Doorway;
			var outputArray = np.zeros(_xSize, _ySize);

			for (int i = 0; i < 4; i++)
			{
				list.Add(filter);
				filter = FeatureFilters.Rotate90CCW(filter);
			}

			// Mark 1 = up, 2 = right, 3 = down, 4 = left
			var convolutions = ConvolveFilters(list, _paddedArray, 1);
			for (int i = 0; i < convolutions.Count(); i++)
			{
				outputArray += FilterArray(convolutions[i], 8, i + 1);
				outputArray += FilterArray(convolutions[i], 6, i + 1);
			}

			return outputArray;
		}

		/// <summary>
		/// By convolving the number of neighbors 2 times each cell has a number indicating how many neighbors
		/// it has in the 64 nearest blocks.
		/// </summary>
		/// <returns></returns>
		NDArray GetDensityMap()
		{
			var conv = ConvolveFilter(FeatureFilters.NeighborCount, 1);
			var padded = GetPaddedArray(conv, 1);
			var density = ConvolveFilter(FeatureFilters.NeighborCount, padded, 1);
			return density;
		}

		public static NDArray[] ConvolveFilters(List<NDArray> ndFilters, NDArray paddedArray, int padding)
		{
			var outputArrays = new NDArray[ndFilters.Count];
			var xSize = paddedArray.shape[0] - (padding * 2);
			var ySize = paddedArray.shape[1] - (padding * 2);
			for(int i = 0; i < ndFilters.Count; i++)
			{
				outputArrays[i] = np.zeros(xSize, ySize);
			}

			var window = ndFilters[0].shape[0];
			var outputArr = np.zeros(xSize, ySize);
			for (int x = 0; x < xSize; x += 1)
			{
				for (int y = 0; y < ySize; y += 1)
				{
					var segment = paddedArray[$"{x}:{x + window},{y}:{y + window}"];
					for(int i = 0; i < outputArrays.Length; i++)
					{
						var conv = np.multiply(segment, ndFilters[i]).sum();
						outputArrays[i][x, y] = conv;
					}
				}
			}
			return outputArrays;
		}

		void CreateNdArrays(int[,] map)
		{
			_xSize = map.GetLength(0);
			_ySize = map.GetLength(1);
			_mapArray = np.array(map);
			_paddedArray = GetPaddedArray(_mapArray, 1);
		}

		public static int[,] GetPaddedArray(NDArray source, int padding)
		{
			var xSize = source.shape[0];
			var ySize = source.shape[1];
			var paddedArray = np.ones(new Shape(xSize + padding * 2, ySize + padding * 2), typeof(int));
			
			for(int x = 0; x < xSize; x++)
			{
				for(int y = 0; y < ySize; y++)
				{
					paddedArray[x+padding,y+padding] = source[x,y];
				}
			}
			return (int[,]) paddedArray.ToMuliDimArray<int>();
		}

		public string ToMapString(NDArray map)
		{
			var intMap = (int[,]) map.ToMuliDimArray<int>();
			return ToMapString(intMap);
		}

		public string ToMapString(int[,] map)
		{
			var one = np.array(new int[]{1});
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
						sb.Append(_mapArray[i,j].Data<int>()[0] == 1 ? "#": "." );
					}
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}
	}
}
