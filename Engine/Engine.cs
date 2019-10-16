using System;
using System.Collections.Generic;
using System.Linq;
using EntityComponentSystemCSharp;
using EntityComponentSystemCSharp.Components;

namespace MD
{
	public class Engine
	{

		const int FLOOR = 0;
		const int WALL = 1;

		Random random = new Random();
		int _width;
		int _height;
		int[,] _floor;
		int[,] _floor_view; // a copy to return to make _floor effectively readonly
		RogueSharp.Map _map;

		EntityManager entityManager = new EntityManager();
		public EntityManager EntityManager => entityManager;
		List<RogueSharp.ICell> _walkable = new List<RogueSharp.ICell>();

		public int[,] Floor
		{
			get{ return _floor_view;}
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
				else
				{
					_walkable.Add(cell);
				}
				_floor[cell.X, cell.Y] = glyph;
			}

			InitializeActors();

			UpdateViews();
		}

		public void InitializeActors()
		{
			var actor = entityManager.CreateEntity();
			actor.AddComponent<ActorComponent>();
			var location = random.Next(0, _walkable.Count);
			var cell = _walkable[location];
			actor.AddComponent(new LocationComponent(){X = cell.X, Y = cell.Y});
		}
		private void UpdateViews()
		{
			if(_floor_view == null)
			{
				_floor_view = new int[_floor.GetLength(0), _floor.GetLength(1)];
			}
			for(int x = 0; x < _floor.GetLength(0); x++ )
			{
				for(int y = 0; y < _floor.GetLength(1); y++)
				{
					_floor_view[x,y] = _floor[x,y];
				}
			}
		}
		private void RandomizeFloor()
		{
			var randomRooms = new RogueSharp.MapCreation.RandomRoomsMapCreationStrategy<RogueSharp.Map>(_width, _height, 20, 10, 5);
			_map = randomRooms.CreateMap();
		}
	}
}
