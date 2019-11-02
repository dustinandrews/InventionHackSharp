using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using NumSharp;


namespace FeatureDetector
{

	[TestFixture]
	public class DetectorTests
	{
		[Test]
		public void ConstructorTest()
		{
			var detector = new Detector(mapIntArray);
		}

		[Test]
		public void FindVerticalWallsTest()
		{
			var detector = new Detector(mapIntArray);
			var walls = detector.FindVerticalEdges();
			Debug.WriteLine(detector.ToMapString(walls));
			Assert.AreEqual(1, walls[12,45]);
		}

		[Test]
		public void FindHorizontalWallsTest()
		{
			var detector = new Detector(mapIntArray);
			var walls = detector.FindHorizontalEdges();
			Assert.AreEqual(1, walls[20,33]);
		}

		[Test]
		public void FindCorridorTest()
		{
			var detector = new Detector(mapIntArray);
			var corridors = detector.FindCorridors();
			Debug.WriteLine(detector.ToMapString(corridors));
			Assert.AreEqual(1, corridors[34,22]);
		}

		[Test]
		public void FindDoorwaysTest()
		{
			var detector = new Detector(mapIntArray);
			var doorways = detector.FindDoorways();
			Debug.WriteLine(detector.ToMapString(doorways));
		}

		private static void IterateMaps(Detector detector, int[,] matrix)
		{
			var thing = detector.ConvolveFilter(matrix);
			var min = (int)thing.min().Data<int>()[0];
			var max = (int)thing.max().Data<int>()[0];
			var arr = (int[,])thing.ToMuliDimArray<int>();
			for (int i = min; i <= max; i++)
			{
				var splats = detector.FilterArray(thing, i);
				Debug.WriteLine(i);
				Debug.WriteLine(detector.ToMapString(splats));
			}
		}

		int[,] mapIntArray = new int[,]{{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}};

	}

	public class Detector
	{
		NDArray _mapArray;
		NDArray _paddedArray;
		int _xSize, _ySize;
		public Detector(int[,] map)
		{
			CreateNdArrays(map);
		}

		public int[,] FindVerticalEdges()
		{
			var filters = new List<int[,]>();
			filters.Add(Matrixes.Vertical);
			var convolution = ConvolveFilters(filters)[0];
			var cutoff = 3;
			return FilterArray(convolution, cutoff);
		}

		public int[,] FindHorizontalEdges()
		{
			var filters = new List<int[,]>();
			filters.Add( Matrixes.RotateMatrixCounterClockwise(Matrixes.Vertical));
			var convolution = ConvolveFilters(filters)[0];
			var cutoff = 3;
			return FilterArray(convolution, cutoff);
		}

		public int[,] FindCorridors()
		{
			var list = new List<int[,]>();
			list.Add(Matrixes.Vertical);
			list.Add(Matrixes.RotateMatrixCounterClockwise(Matrixes.Vertical));
			var convolutions = ConvolveFilters(list);
			var outputArray = np.zeros(_xSize, _ySize);
			foreach(NDArray conv in convolutions)
			{
				var corridors = FilterArray(conv, -6);
				outputArray += np.array(corridors);
			}
			return  (int[,]) outputArray.ToMuliDimArray<int>();
		}

		
		public int[,] FindDoorways()
		{
			var list = new List<int[,]>();
			var filter = Matrixes.Doorway;
			for(int i = 0; i < 4; i ++)
			{
				list.Add(filter);
				filter = Matrixes.RotateMatrixCounterClockwise(filter);
			}
			var convolutions = ConvolveFilters(list);
			var outputArray = np.zeros(_xSize, _ySize);
			foreach(NDArray conv in convolutions)
			{
				outputArray += FilterArray(conv, -8);
			}
			return (int[,]) outputArray.ToMuliDimArray<int>();
		}

		public int[,] FilterArray(NDArray array, int match)
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
			return (int[,]) outArray.ToMuliDimArray<int>();
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
			_paddedArray = np.zeros(new Shape(_xSize + 2, _ySize + 2), typeof(int));
			
			for(int x = 0; x < _xSize; x++)
			{
				for(int y = 0; y < _ySize; y++)
				{
					_paddedArray[x+1,y+1] = _mapArray[x,y];
				}
			}
		}

		public string ToMapString(int[,] map)
		{
			var one = np.array(new int[]{1});
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for(int i = 0; i < map.GetLength(0); i++)
			{
				for(int j = 0; j < map.GetLength(1); j++)
				{
					if(map[i,j] == 1)
					{
						sb.Append("*");
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

	public static class Matrixes
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
