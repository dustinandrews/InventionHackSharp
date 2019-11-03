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

		public int[,] FindVerticalWalls()
		{
			var filters = new List<int[,]>();
			filters.Add(FeatureFilters.Vertical);
			var convolution = ConvolveFilters(filters)[0];
			Debug.WriteLine(convolution.ToString());
			// 22,24,25,26,32
			var match = FilterArray(convolution, 22);
			match += FilterArray(convolution, 24);
			match += FilterArray(convolution, 25);
			match += FilterArray(convolution, 26);
			match += FilterArray(convolution, 32);
			return (int[,]) match.ToMuliDimArray<int>();
		}

		public int[,] FindHorizontalWalls()
		{
			var filters = new List<int[,]>();
			filters.Add( FeatureFilters.RotateMatrixCounterClockwise(FeatureFilters.Vertical));
			var convolution = ConvolveFilters(filters)[0];
			Debug.WriteLine(convolution.ToString());
			var match = FilterArray(convolution, 22);
			match += FilterArray(convolution, 24);
			match += FilterArray(convolution, 25);
			match += FilterArray(convolution, 26);
			match += FilterArray(convolution, 32);
			return (int[,]) match.ToMuliDimArray<int>();
		}

		public int[,] FindCorridors()
		{
			var list = new List<int[,]>();
			list.Add(FeatureFilters.Vertical);
			list.Add(FeatureFilters.RotateMatrixCounterClockwise(FeatureFilters.Vertical));
			var convolutions = ConvolveFilters(list);
			var outputArray = np.zeros(_xSize, _ySize);
			foreach(NDArray conv in convolutions)
			{
				var corridors = FilterArray(conv, -10);
				corridors += FilterArray(conv, -11);
				corridors += FilterArray(conv, -12);
				outputArray += np.array(corridors);
			}
			var convCross = ConvolveFilter(FeatureFilters.Cross);
			outputArray += FilterArray(convCross, 30);
			outputArray += FilterArray(convCross, 31);
			outputArray += FilterArray(convCross, 32);
			var intOutPutArray = (int[,]) outputArray.ToMuliDimArray<int>();
			intOutPutArray.UpDate(e => (e > 0) ? 1 : 0);
			Debug.WriteLine(ToMapString(intOutPutArray));
			return  intOutPutArray;
		}

		public int[,] FindDoorways()
		{
			var list = new List<int[,]>();
			var filter = FeatureFilters.Doorway;
			for(int i = 0; i < 4; i ++)
			{
				list.Add(filter);
				filter = FeatureFilters.RotateMatrixCounterClockwise(filter);
			}
			var convolutions = ConvolveFilters(list);
			var outputArray = np.zeros(_xSize, _ySize);
			foreach(NDArray conv in convolutions)
			{
				outputArray += FilterArray(conv, 8);
			}
			return (int[,]) outputArray.ToMuliDimArray<int>();
		}

		public int[,] FindCorners()
		{
			var list = new List<int[,]>();
			var filter = FeatureFilters.InnerCorner;
			for(int i = 0; i < 4; i ++)
			{
				list.Add(filter);
				filter = FeatureFilters.RotateMatrixCounterClockwise(filter);
			}

			filter = FeatureFilters.OuterCorner;
			for(int i = 0; i < 4; i ++)
			{
				list.Add(filter);
				filter = FeatureFilters.RotateMatrixCounterClockwise(filter);
			}
			var convolutions = ConvolveFilters(list);
			var outputArray = np.zeros(_xSize, _ySize);
			foreach(NDArray conv in convolutions)
			{
				outputArray += FilterArray(conv, 8);
			}
			return (int[,]) outputArray.ToMuliDimArray<int>();
		}

		public NDArray FilterArray(NDArray array, int match)
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
						outArray[x,y] = 1;
					}
				}
			}
			//return (int[,]) outArray.ToMuliDimArray<int>();
			return outArray;
		}
		
		public NDArray ConvolveFilter(int[,] filter)
		{
			var list = new List<int[,]>();
			list.Add(filter);
			return ConvolveFilters(list)[0];
		}

		public NDArray[] ConvolveFilters(List<int[,]> filters)
		{
			var ndList = new List<NDArray>();
			foreach(var f in filters)
			{
				ndList.Add(np.array(f));
			}

			return ConvolveFilters(ndList);
		}

		NDArray[] ConvolveFilters(List<NDArray> ndFilters)
		{
			var outputArrays = new NDArray[ndFilters.Count];
			for(int i = 0; i < ndFilters.Count; i++)
			{
				outputArrays[i] = np.zeros(_xSize, _ySize);
			}

			var window = ndFilters[0].shape[0];
			var outputArr = np.zeros(_xSize, _ySize);
			for (int x = 0; x < _xSize; x += 1)
			{
				for (int y = 0; y < _ySize; y += 1)
				{
					var segment = _paddedArray[$"{x}:{x + window},{y}:{y + window}"];
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
			_paddedArray = np.ones(new Shape(_xSize + 2, _ySize + 2), typeof(int));
			
			for(int x = 0; x < _xSize; x++)
			{
				for(int y = 0; y < _ySize; y++)
				{
					_paddedArray[x+1,y+1] = _mapArray[x,y];
				}
			}
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
