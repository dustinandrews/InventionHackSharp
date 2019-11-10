using System.Diagnostics;
using RogueSharp;
using FeatureDetector;
using RogueSharp.MapCreation;
using Engine;
using System;
using RoyT.AStar;
using System.Collections.Generic;
using static FeatureDetector.MapFeatureDetector;
using System.Linq;

namespace MegaDungeon
{
	public class CaveMapCreationStrategy<T> : IMapCreationStrategy<T> where T : IMap, new()
	{
		Random random = new Random();
		int _width;
		int _height;
		double _density;
		public CaveMapCreationStrategy(int width, int height, double density = 0.50)
		{
			_width = width;
			_height = height;
			_density = density;
		}

		public T CreateMap()
		{
			var cave = CellularAutomata.GenerateCaves(_width, _height, _density);
			var regionMap = MapFeatureDetector.GetRegions(cave);

			// Pick a random room to start and dig a tunnel to a random other room.
			// Go from there to another random room until all are connected.
			var roomCenters = GetRoomCenters(regionMap).ToList();
			var startPoint = roomCenters[random.Next(roomCenters.Count)];
			roomCenters.Remove(startPoint);
			
			while(roomCenters.Count > 0)
			{
				var nextPoint = roomCenters[random.Next(roomCenters.Count)];
				roomCenters.Remove(nextPoint);
				cave = CreatePath(cave, startPoint.X, startPoint.Y, nextPoint.X, nextPoint.Y);
				startPoint = nextPoint;
			}
			var map = new RogueSharp.Map(_width, _height);
			foreach(var cell in map.GetAllCells())
			{
				var open = cave[cell.X, cell.Y] == 0;
				map.SetCellProperties(cell.X, cell.Y, isTransparent: open, isWalkable: open, false);
			}

			return (T)(object)map;
		}

		private int[,] CreatePath(int[,] map, int x1, int y1, int x2, int y2)
		{
			int[,] returnMap = (int[,]) map.Clone();
			Grid grid = CreateGrid(map);
			var start = new Position(x1, y1);
			var end = new Position(x2, y2);
			var path = grid.GetPath(start, end, MovementPatterns.LateralOnly);
			foreach(var p in path)
			{
				returnMap[p.X, p.Y] = 0;
			}
			return returnMap;
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
