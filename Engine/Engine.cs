using System;
using System.Collections.Generic;
using System.Linq;
using EntityComponentSystemCSharp;
using EntityComponentSystemCSharp.Components;
using EntityComponentSystemCSharp.Systems;
using RogueSharp;
using MegaDungeon.Contracts;

namespace MegaDungeon
{
    public class Engine
	{

		const int FLOOR = 848;
		const int VERTICALWALL = 830;
		const int HORIZWALL = 834;
		const int INNERCORNERWALL = 832;
		const int OUTERCORNERWALL = 832;
		const int DARK = 829;
		const int DOOR = 844;
		const int PLAYER = 340;

		Random random = new Random();
		int _width;
		int _height;
		int[,] _floor;
		int[,] _floor_view; // a copy to return to make _floor effectively readonly
		RogueSharp.Map _map;
		ITileManager _tileManager;

		Point UP = new Point(0,-1);
		Point UPRIGHT = new Point(1, -1);
		Point RIGHT = new Point(1, 0);
		Point DOWNRIGHT = new Point(1, 1);
		Point DOWN = new Point(0, 1); 
		Point DOWNLEFT = new Point(-1, 1);
		Point LEFT = new Point(-1, 0);
		Point UPLEFT = new Point(-1, -1);
		Point CENTER = new Point(0,0);
		Point[] _compassPoints;
		List<int[]> _doorways = new List<int[]>();
		Dictionary<PlayerInput, Point> _cardinalVectors;

		EntityManager entityManager = new EntityManager();
		public EntityManager EntityManager => entityManager;
		List<RogueSharp.ICell> _walkable = new List<RogueSharp.ICell>();
		List<ISystem> _turnSystems = new List<ISystem>();

		public int[,] Floor
		{
			get{ return _floor_view;}
		}

		public Engine(int width, int height, ITileManager tileManager)
		{
			_width = width;
			_height = height;
			_floor = new int[width, height];

			_tileManager = tileManager;

			_cardinalVectors = new Dictionary<PlayerInput, Point>()
			{
				{PlayerInput.UP, UP},
				{PlayerInput.UPRIGHT, UPRIGHT},
				{PlayerInput.RIGHT, RIGHT},
				{PlayerInput.DOWNRIGHT, DOWNRIGHT},
				{PlayerInput.DOWN, DOWN},
				{PlayerInput.DOWNLEFT, DOWNLEFT},
				{PlayerInput.LEFT, LEFT},
				{PlayerInput.UPLEFT, UPLEFT}
			};

			_compassPoints = new [] {
				UPLEFT, UP, UPRIGHT,
				LEFT, CENTER, RIGHT,
				DOWNLEFT, DOWN, DOWNRIGHT
			};

			_doorways.AddRange(new [] {Filters.DoorWay, Filters.DoorWay2, Filters.DoorWay3, Filters.DoorWay4});

			RandomizeFloor();
			
			foreach(var cell in _map.GetAllCells())
			{
				var sample = GetCellFilterArray(cell);
				var glyph = DARK;
				var stotal = sample.Total(); 

				if(sample.Total() < 9)
				{
					if(!cell.IsWalkable)
					{
						// Total number of filled in blocks in the 3x3 area including this cell
						 if(stotal == 8)
						{
							glyph = INNERCORNERWALL;
						}
						else if(stotal == 4 || stotal == 5)
						{
							glyph = OUTERCORNERWALL;
						}
						else if(FilterMatch(sample, Filters.Horizontal))
						{
							glyph = HORIZWALL;
						}
						else if(FilterMatch(sample, Filters.Vertical))
						{
							glyph = VERTICALWALL;
						}
						else if(MultiplyFilter(sample, Filters.Horizontal).Total() == 1)
						{
							glyph = HORIZWALL;
						}
						else if(MultiplyFilter(sample, Filters.Vertical).Total() == 1)
						{
							glyph = VERTICALWALL;
						}
					}
					else
					{
						glyph = FLOOR;
						if(stotal == 4)
						{
							foreach(var doorway in _doorways)
							{
								if(FilterMatch(sample, doorway))
								{
									glyph = DOOR;
									break;
								}
							}
						}

						_walkable.Add(cell);
					}
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
			actor.AddComponent(new GlyphComponent{glyph = "@"});
		}

		public void DoTurn(PlayerInput playerInput)
		{
			var player = entityManager.GetAllEntitiesWithComponent<PlayerComponent>().FirstOrDefault();
			if(player != null)
			{
				var position = player.GetComponent<LocationComponent>();
				if(_cardinalVectors.ContainsKey(playerInput))
				{
					var delta = _cardinalVectors[playerInput];
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

		bool FilterMatch(int[] sample, int[] filter)
		{
			var total = FilterTotal(sample, filter);
			var ftotal = filter.Total();
			return total == ftotal;
		}
		int[] GetCellFilterArray(ICell cell)
		{
			var outArray = new int[_compassPoints.Length];
			for(int i =0 ; i <_compassPoints.Length; i++)
			{
				var point = new Point(cell.X, cell.Y) + _compassPoints[i];
				outArray[i] = 1;

				if(point.X > -1 && point.X < _width && point.Y > -1 && point.Y < _height)
				{
					if(_map.GetCell(point.X, point.Y).IsWalkable)
					{
						outArray[i] = 0;
					}
				}
			}
			return outArray;
		}

		int[] MultiplyFilter(int[] sample, int[] filter)
		{
			if(sample.Length != filter.Length)
			{
				throw new ArgumentException("sample and filter must be the same size.");
			}
			var outArray = new int[filter.Length];
			for(int i = 0; i < filter.Length; i++)
			{
				outArray[i] = sample[i] * filter[i];
			}
			return outArray;
		}

		int FilterTotal(int[] sample, int[] filter)
		{
			var mult = MultiplyFilter(sample, filter);
			return mult.Total();
		}

		/// <summary>
		/// Copy the private _floor to the public _floor_view
		/// </summary>
		void UpdateViews()
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
