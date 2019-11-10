using NUnit.Framework;
using FeatureDetector;
using MegaDungeon;
using System.Diagnostics;

namespace Engine
{
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

		[Test]
		public void CaveTest()
		{
			var strat = new CaveMapCreationStrategy<RogueSharp.Map>(100, 120, 0.65);
			var map = strat.CreateMap();
			Debug.WriteLine(map.ToString());
		}

	}

}