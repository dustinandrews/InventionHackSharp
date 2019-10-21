using System;
using System.Linq;
using System.Collections.Generic;
using EntityComponentSystemCSharp.Components;
using EntityComponentSystemCSharp;
using static EntityComponentSystemCSharp.EntityManager;
using Inv;

namespace Portable
{
	public class MegaDungeonUI
	{
		int _horizontalCellCount;
		int _verticalCellCount;
		Surface _surface;
		MegaDungeon.Engine _engine;
		MegaDungeon.PlayerInput _lastInput = MegaDungeon.PlayerInput.NONE;
		TileManager _tileManager;
		Dictionary<Entity, LocationComponent> _lastLocation = new Dictionary<Entity, LocationComponent>();
		Dictionary<int, int> _actorLocationMap = new Dictionary<int, int>();
		Cloth _cloth;
		Point _player;

		/// <summary>
		/// Mapping from UI implementation to game.
		/// </summary>
		/// <remarks>
		/// Enforce a strong firewall between the game the UI implmentation.
		/// </remarks>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="MD.PlayerInput"></typeparam>
		/// <returns></returns>
		public Dictionary<Inv.Key,MegaDungeon.PlayerInput> keyMap = new Dictionary<Key,MegaDungeon.PlayerInput>()
		{
			{Inv.Key.n1,MegaDungeon.PlayerInput.DOWNLEFT},
			{Inv.Key.n2,MegaDungeon.PlayerInput.DOWN},
			{Inv.Key.n3,MegaDungeon.PlayerInput.DOWNRIGHT},
			{Inv.Key.n4,MegaDungeon.PlayerInput.LEFT},
			{Inv.Key.n5,MegaDungeon.PlayerInput.NONE},
			{Inv.Key.n6,MegaDungeon.PlayerInput.RIGHT},
			{Inv.Key.n7,MegaDungeon.PlayerInput.UPLEFT},
			{Inv.Key.n8,MegaDungeon.PlayerInput.UP},
			{Inv.Key.n9,MegaDungeon.PlayerInput.UPRIGHT},
			{Inv.Key.Up,MegaDungeon.PlayerInput.UP},
			{Inv.Key.Down,MegaDungeon.PlayerInput.DOWN},
			{Inv.Key.Left,MegaDungeon.PlayerInput.LEFT},
			{Inv.Key.Right,MegaDungeon.PlayerInput.RIGHT},
		};

		public MegaDungeon.PlayerInput LastInput { get => _lastInput; set => _lastInput = value; }

		/// <summary>
		/// Constructor for Main UI logic class.
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="horizontalCellCount"></param>
		/// <param name="vericalCellCount"></param>
		public MegaDungeonUI(Surface surface, int horizontalCellCount, int vericalCellCount)
		{
			_tileManager =  new TileManager("absurd64.bmp", System.IO.File.ReadAllText("tiledata.json"));
			_horizontalCellCount = horizontalCellCount;
			_verticalCellCount = vericalCellCount;
			_surface = surface;
			_cloth = CreateCloth();
			_surface.Content = _cloth;
			_cloth.Draw();
			_surface.ComposeEvent += ()=>
			{
				Update();
			};
			_engine = new MegaDungeon.Engine(_horizontalCellCount,_verticalCellCount, _tileManager);
		}

		/// <summary>
		/// Update one atomic unit of the UI.
		/// </summary>
		/// <remarks>
		/// This method is the pump for the game. It should be fast enough to always return in less than
		/// one frame (1/60th sec.)
		/// </remarks>
		public void Update()
		{
			if(_lastInput != MegaDungeon.PlayerInput.NONE)
			{
				_engine.DoTurn(_lastInput);
				_lastInput = MegaDungeon.PlayerInput.NONE;
				GetActorsFromEngine();
				Update();
				_cloth.Draw();
			}
		}

		public void AcceptInput(Inv.Keystroke keystroke)
		{
			if(keyMap.ContainsKey(keystroke.Key))
			{
				_lastInput = keyMap[keystroke.Key];
			}
		}

		internal Cloth CreateCloth()
		{
			var cloth = new Cloth();
			cloth.Dimension = new Inv.Dimension(_horizontalCellCount, _verticalCellCount); // 35 x 40
			cloth.CellSize = _surface.Window.Width / _horizontalCellCount;

			cloth.DrawEvent += (DC, Patch) =>
			{
				int glyph;
				if(_actorLocationMap.ContainsKey(Patch.X + (Patch.Y * _horizontalCellCount)))
				{
					glyph = _actorLocationMap[Patch.X + (Patch.Y * _horizontalCellCount)];
				}
				else
				{
					glyph = _engine.Floor[Patch.X, Patch.Y];
				}
				
				var bytes = _tileManager.GetTileBmpBytes(glyph);
				var image = _tileManager.GetInvImage(glyph);
				DC.DrawImage(image, Patch.Rect);
			};
			return cloth;
		}

			void GetActorsFromEngine()
			{
				foreach(var actor in _engine.EntityManager.GetAllEntitiesWithComponent<LocationComponent>())
				{

					var location = actor.GetComponent<LocationComponent>();
					var glyph = actor.GetComponent<GlyphComponent>();

					if(!_lastLocation.ContainsKey(actor))
					{
						_lastLocation.Add(actor, location.Clone());
						_actorLocationMap.Add(location.X + (location.Y * _horizontalCellCount), glyph.glyph);
					}

					var last = _lastLocation[actor];
					if(last != location)
					{
						_actorLocationMap.Remove(last.X + (last.Y * _horizontalCellCount));
						_actorLocationMap.Add(location.X + (location.Y * _horizontalCellCount), glyph.glyph);
					}

					last.X = location.X;
					last.Y = location.Y;

					var player = actor.GetComponent<PlayerComponent>();
					if(player != null)
					{
						_player = new Point(location.X, location.Y);
					}
				}
			}
	}
}
