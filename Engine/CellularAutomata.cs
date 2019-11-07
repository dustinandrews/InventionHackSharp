using System;
using System.Linq;
using NUnit.Framework;
using FeatureDetector;
using System.Diagnostics;

namespace Engine
{
	public static class CellularAutomata
	{
		static Random _rand = new Random();

		public static int[,] RandomFill(int width, int height, double fill = 0.5F)
		{
			var grid = new int[width, height];
			grid.UpDate(c => c = (_rand.NextDouble() < fill)? 1 : 0);
			return grid;
		}


		public static int[,] Convolve(int[,] grid, int[,] filter)
		{
			return FeatureDetector.MapFeatureDetector.ConvolveFilter(grid, filter);
		}

		public static int[,] RunAutomataTimestep(int[,] grid, int[] born, int[] survive)
		{
			var newgrid = new int[grid.GetLength(0), grid.GetLength(1)];
				var neighbors = CellularAutomata.Convolve(grid, FeatureFilters.NeighborCount);
				Debug.WriteLine(neighbors.Total());
				for (int x = 0; x < grid.GetLength(0); x++)
				{
					for (int y = 0; y < grid.GetLength(1); y++)
					{
						if (grid[x, y] == 1 && survive.Contains(neighbors[x, y]))
						{
							newgrid[x, y] = 1;
						}
						else if (grid[x, y] == 0 && born.Contains(neighbors[x, y]))
						{
							newgrid[x, y] = 1;
						}
						else
						{
							newgrid[x, y] = 0;
						}
					}
				}
			return newgrid;
		}

		public static int[,] GenerateCaves(int width, int height, double density = 0.45)
		{
			// caves B678/S345678
			var grid = CellularAutomata.RandomFill(width, height, density);

			int[] born = new[] { 6,7,8 };
			int[] survive = new[] { 3, 4, 5, 6, 7, 8 };

			for (int i = 0; i < 5; i++)
			{
				grid = CellularAutomata.RunAutomataTimestep(grid, born, survive);
			}

			return grid;
		}


	}

	[TestFixture]
	public class CelluarAutomataTests
	{


		[Test]
		public void RandomFillTest()
		{
			var w = 10;
			var h = 10;
			var total = 0;
			for (int i = 0; i < 100; i++)
			{
				var result = CellularAutomata.RandomFill(h, w);
				total += result.Total();
			}
			var expected = w * h / 2;
			var avg = total / 100;
			var epsilon = w * h / 4;
			Assert.AreEqual(expected, avg, epsilon);
		}

		[Test]
		public void ConvoleTest()
		{

			var grid = CellularAutomata.RandomFill(10, 15);
			var conv = CellularAutomata.Convolve(grid, FeatureFilters.Identity);
			var gTotal = grid.Total();
			var cTotal = conv.Total();
			Assert.AreEqual(gTotal, cTotal);
		}

		[Test]
		public void TimeSteptest()
		{
			var w = 100;
			var h = 150;
			var grid = CellularAutomata.RandomFill(w, h, 0.45);

			// caves B678/S345678
			int[] born = new[] { 6,7,8 };
			int[] survive = new[] { 3, 4, 5, 6, 7, 8 };

			for (int i = 0; i < 5; i++)
			{
				grid = CellularAutomata.RunAutomataTimestep(grid, born, survive);
				Debug.WriteLine(grid.ToRowString(asMap: true));
			}
		}
	}
}