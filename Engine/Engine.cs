using System;
using System.Collections.Generic;
using System.Linq;

namespace MD
{
	public class Engine
	{
		static int FLOOR = 0;
		static int WALL = 1;
		int _width;
		int _height;
		int[,] _floor;
		RogueSharp.Map _map;

		public int[,] Floor
		{
			get{ return (int[,])_floor.Clone();}
		}
		public Engine(int width, int height)
		{
			_width = width;
			_height = height;
			_floor = new int[width, height];
			RandomizeFloor();
			foreach(var cell in _map.GetAllCells())
			{
				var glyph = FLOOR;
				if(!cell.IsWalkable)
				{
					glyph = WALL;
				}
				_floor[cell.X, cell.Y] = glyph;
			}
		}

		private void RandomizeFloor()
		{
			var randomRooms = new RogueSharp.MapCreation.RandomRoomsMapCreationStrategy<RogueSharp.Map>(_width, _height, 20, 10, 5);
			_map = randomRooms.CreateMap();
		}
	}
}
