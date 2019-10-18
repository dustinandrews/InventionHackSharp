using System;
using System.Collections.Generic;
using System.Linq;
using EntityComponentSystemCSharp;
using EntityComponentSystemCSharp.Components;
using EntityComponentSystemCSharp.Systems;
using RogueSharp;

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

		Dictionary<PlayerInput, Point> CardinalVectors = new Dictionary<PlayerInput, Point>()
		{
			{PlayerInput.UP, new Point(0, -1)},
			{PlayerInput.UPRIGHT, new Point(1,-1)},
			{PlayerInput.RIGHT, new Point(1, 0)},
			{PlayerInput.DOWNRIGHT, new Point(-1, 1)},
			{PlayerInput.DOWN, new Point(0, 1)},
			{PlayerInput.DOWNLEFT, new Point(-1, 1)},
			{PlayerInput.LEFT, new Point(-1, 0)},
			{PlayerInput.UPLEFT, new Point(-1, -1)}
		};

		EntityManager entityManager = new EntityManager();
		public EntityManager EntityManager => entityManager;
		List<RogueSharp.ICell> _walkable = new List<RogueSharp.ICell>();
		List<ISystem> _turnSystems = new List<ISystem>();

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

			InitializePlayer();

			UpdateViews();

			// Add systems that should run every turn here.
			_turnSystems.Add(new MovementSystem(entityManager));
		}

		/// <summary>
		/// Add the player to the map.
		/// </summary>
		public void InitializePlayer()
		{
			var actor = entityManager.CreateEntity();
			actor.AddComponent<ActorComponent>();
			actor.AddComponent<PlayerComponent>();
			var location = random.Next(0, _walkable.Count);
			var cell = _walkable[location];
			actor.AddComponent(new LocationComponent(){X = cell.X, Y = cell.Y});
			var mapComponent = new MapComponent(){map = _map};
			actor.AddComponent(mapComponent);
		}

		public void DoTurn(PlayerInput playerInput)
		{
			var player = entityManager.GetAllEntitiesWithComponent<PlayerComponent>().FirstOrDefault();
			if(player != null)
			{
				var position = player.GetComponent<LocationComponent>();
				if(CardinalVectors.ContainsKey(playerInput))
				{
					var delta = CardinalVectors[playerInput];
					var desired = delta + new Point(position.X, position.Y);
					var desiredComp = player.GetComponent<DestinationComponent>();
					if(desiredComp == null)
					{
						desiredComp = new DestinationComponent();
						player.AddComponent(desiredComp);
					}
					desiredComp.X = desired.X;
					desiredComp.Y = desired.Y;
				}
			}

			// Run all the game systems.
			foreach(var system in _turnSystems)
			{
				system.Run();
			}
		}

		/// <summary>
		/// Copy the private _floor to the public _floor_view
		/// </summary>
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
