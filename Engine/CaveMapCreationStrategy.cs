using System.Diagnostics;
using RogueSharp;
using FeatureDetector;
using RogueSharp.MapCreation;
using Engine;
using System;
using RoyT.AStar;

namespace MegaDungeon
{
	public class CaveMapCreationStrategy<T> : IMapCreationStrategy<T> where T : IMap, new()
	{
		int _width;
		int _height;
		double _density;
		public CaveMapCreationStrategy(int width, int height, double density = 0.45)
		{
			_width = width;
			_height = height;
			_density = density;
		}

		public T CreateMap()
		{
			var cave = CellularAutomata.GenerateCaves(_width, _height, _density);

			var bigroom = new RogueSharp.MapCreation.BorderOnlyMapCreationStrategy<RogueSharp.Map>(_width, _height);
			var map = bigroom.CreateMap();
			Grid pathfinder = CreateGrid(cave);

			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					if (cave[x, y] == 0)
					{
						//var path = pathfinder.GetPath(midCell, new RoyT.AStar.Position(x, y), RoyT.AStar.MovementPatterns.LateralOnly);
					}
				}
			}

			Debug.WriteLine(cave.ToRowString(true));
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					var open = cave[x, y] == 0;
					map.SetCellProperties(x, y, open, open, false);
				}
			}

			return (T)(object)map;
		}

		private Grid CreateGrid(int[,] cave)
		{
			var pathfinder = new RoyT.AStar.Grid(_width, _height);
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					pathfinder.SetCellCost(new RoyT.AStar.Position(x, y), (cave[x, y] + 1) * 10);
				}
			}

			return pathfinder;
		}

		
	}
}
