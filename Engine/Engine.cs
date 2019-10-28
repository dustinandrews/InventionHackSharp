using System;
using System.Collections.Generic;
using System.Linq;
using EntityComponentSystemCSharp;
using EntityComponentSystemCSharp.Components;
using EntityComponentSystemCSharp.Systems;
using RogueSharp;
using MegaDungeon.Contracts;
using static MegaDungeon.EngineConstants;

namespace MegaDungeon
{
    public class Engine
	{
		Random random = new Random();
		int _width;
		int _height;
		int[,] _floor;
		int[,] _floor_view; // a copy to return to make _floor effectively readonly
		RogueSharp.Map _map;
		ITileManager _tileManager;
		Location _playerLocation = new Location();
		List<int[]> _doorways = new List<int[]>();
		EntityManager entityManager = new EntityManager();
		public EntityManager EntityManager => entityManager;
		List<RogueSharp.ICell> _walkable = new List<RogueSharp.ICell>();
		List<ISystem> _turnSystems = new List<ISystem>();
		Queue<string> _messages = new Queue<string>();
		int _messageLimit = 5;
		Sight _playerSiteDistance;
		EntityManager.Entity _player;
		ActorManager _actorManager;
		HashSet<Point> _viewable;

		public HashSet<Point> Viewable
		{
			get => _viewable;
		}

		public EntityManager.Entity PlayerEntity
		{
			get => _player;
		}

		public Point PlayerLocation
		{
			get => new Point(_playerLocation.X, _playerLocation.Y);
		}

		public string[] Messages
		{
			get => _messages.ToArray();
		}

		/// <summary>
		/// A grid of glyphs representing the discovered map.
		/// </summary>
		/// <value></value>
		public int[,] Floor
		{
			get{ return _floor_view;}
		}

		/// <summary>
		/// Dungeon game engine, seperate from any UI.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="tileManager"></param>
		public Engine(int width, int height, ITileManager tileManager)
		{
			_width = width;
			_height = height;
			_floor = new int[width, height];
			_tileManager = tileManager;

			RandomizeFloor();
			InitCellGlyphs();
			_actorManager = new ActorManager(entityManager);
			InitializePlayer();
			PlaceMonsters();
			UpdateViews();
			// Add systems that should run every turn here.
			_turnSystems.Add(new MovementSystem(entityManager, _map));
		}

		/// <summary>
		/// Given the player input, advance the game one turn.
		/// </summary>
		/// <param name="playerInput"></param>
		public void DoTurn(PlayerInput playerInput)
		{
			var player = entityManager.GetAllEntitiesWithComponent<Player>().FirstOrDefault();
			if(player != null)
			{
				var position = player.GetComponent<Location>();
				if(INPUTMAP.ContainsKey(playerInput))
				{
					var delta = INPUTMAP[playerInput];
					var desired = delta + new Point(position.X, position.Y);
					var desiredComp = player.GetComponent<Destination>();
					if(desiredComp == null)
					{
						desiredComp = new Destination();
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

			_messages.Enqueue(playerInput.ToString());
			while(_messages.Count() > _messageLimit)
			{
				_messages.Dequeue();
			}

			UpdateViews();
		}


		void InitializePlayer()
		{
			_player = _actorManager.GetPlayerActor();
			ICell cell = GetWalkableCell();
			_playerSiteDistance = _player.GetComponent<Sight>();
			_playerLocation = new Location() { X = cell.X, Y = cell.Y };
			_player.AddComponent(_playerLocation);
		}

		ICell GetWalkableCell()
		{
			var location = random.Next(0, _walkable.Count);
			var cell = _walkable[location];
			_walkable.Remove(cell);
			return cell;
		}

		void PlaceMonsters()
		{
			var numMonsters = RogueSharp.DiceNotation.Dice.Roll("1D6+3");
			for(int i = 0; i < numMonsters; i++)
			{
				var location = GetWalkableCell();
				var monster = _actorManager.CreateActor(60);
				monster.AddComponent(new Location(){X = location.X, Y = location.Y});
				monster.AddComponent(new Faction(){Type = Factions.Monster});
				_messages.Enqueue(location.ToString());
			}
		}

		/// <summary>
		/// Examine each cell and pick a suitable glyph for walls.
		/// </summary>
		void InitCellGlyphs()
		{
			foreach (var cell in _map.GetAllCells())
			{
				var sample = Filters.GetCellFilterArray(cell, _map);
				var glyph = DARK;
				var stotal = sample.Total();

				if (sample.Total() < 9) // 9 == all surounding cells impassible
				{
					if (!cell.IsWalkable)
					{
						// Total number of filled in blocks in the 3x3 area including this cell
						if (stotal == 8)
						{
							glyph = INNERCORNERWALL;
						}
						else if (stotal == 4 || stotal == 5)
						{
							glyph = OUTERCORNERWALL;
						}
						else if (Filters.FilterMatch(sample, Filters.Horizontal))
						{
							glyph = HORIZWALL;
						}
						else if (Filters.FilterMatch(sample, Filters.Vertical))
						{
							glyph = VERTICALWALL;
						}
						else if (Filters.MultiplyFilter(sample, Filters.Horizontal).Total() == 1)
						{
							glyph = HORIZWALL;
						}
						else if (Filters.MultiplyFilter(sample, Filters.Vertical).Total() == 1)
						{
							glyph = VERTICALWALL;
						}
					}
					else
					{
						glyph = FLOOR;

						// Store intial walkable cells in order to place new entities in valid locations.
						_walkable.Add(cell);
					}
				}
				_floor[cell.X, cell.Y] = glyph;
			}
		}

		/// <summary>
		/// Copy the private _floor to the public _floor_view
		/// </summary>
		void UpdateViews()
		{
			var circleView = GetFieldOfView(_playerLocation.X, _playerLocation.Y, _playerSiteDistance.Range, _map);
			var viewable = new List<Point>();
			foreach(var cell in circleView)
			{
				viewable.Add(new Point(cell.X, cell.Y));
				_map.AppendFov(cell.X, cell.Y, 0, true);
			}
			_viewable = new HashSet<Point>(viewable);

			if(_floor_view == null)
			{
				_floor_view = new int[_floor.GetLength(0), _floor.GetLength(1)];
			}

			for(int x = 0; x < _floor.GetLength(0); x++ )
			{
				for(int y = 0; y < _floor.GetLength(1); y++)
				{
					if(!_map.IsInFov(x, y))
					{
						_floor_view[x,y] = DARK;
					}
					else
					{
						_floor_view[x,y] = _floor[x,y];
					}
				}
			}
		}

		private static IEnumerable<ICell> GetFieldOfView( int x, int y, int selectionSize, IMap map )
		{
		List<ICell> circleFov = new List<ICell>();
		var fieldOfView = new FieldOfView( map );
		var cellsInFov = fieldOfView.ComputeFov( x, y, (int) ( selectionSize * 1.5 ), true );
		var circle = map.GetCellsInCircle( x, y, selectionSize ).ToList();
		foreach ( ICell cell in cellsInFov )
		{
			if ( circle.Contains( cell ) )
			{
			circleFov.Add( cell );
			}
		}
		return circleFov;
		}

		private void RandomizeFloor()
		{
			var randomRooms = new RogueSharp.MapCreation.RandomRoomsMapCreationStrategy<RogueSharp.Map>(_width, _height, 20, 10, 5);
			_map = randomRooms.CreateMap();
		}
	}
}
