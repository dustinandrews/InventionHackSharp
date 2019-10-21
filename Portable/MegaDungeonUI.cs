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
		Label[,] _labels;

		IEnumerator<object> enumerator = null;
		bool IsInit = false;
		bool IsMapChanged = true;
		Stack _root_stack;
		MegaDungeon.Engine _engine;

		MegaDungeon.PlayerInput _lastInput = MegaDungeon.PlayerInput.NONE;
		TileManager _tileManager;

		Dictionary<Entity, LocationComponent> _lastLocation = new Dictionary<Entity, LocationComponent>();

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

		public MegaDungeonUI(Surface surface, int horizontalCellCount, int vericalCellCount)
		{
			_tileManager =  new TileManager("absurd64.bmp", System.IO.File.ReadAllText("tiledata.json"));
			_horizontalCellCount = horizontalCellCount;
			_verticalCellCount = vericalCellCount;
			_surface = surface;
			_engine = new MegaDungeon.Engine(_horizontalCellCount,_verticalCellCount, _tileManager);
		}

		/// <summary>
		/// Populate the map surface with the map elements
		/// </summary>
		/// <remarks>
		/// This implementation creates a grid of labels. For small maps this works fine
		/// and is easy to manipulate. Larger maps will suffer performance problems and the
		/// map should be implmented as an image.
		/// </remarks>
		/// <param name="surface"></param>
		public void InitializeSurface(Surface surface)
		{
			surface.Background.Colour = Colour.WhiteSmoke;
			_root_stack = surface.NewStack(Orientation.Vertical);
			surface.Content = _root_stack;
			_root_stack.Background.Colour = Colour.DarkGray;
			_root_stack.Size.Set(surface.Window.Width, surface.Window.Height);
			_labels = new Label[_horizontalCellCount, _verticalCellCount];
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
			if(!IsInit)
			{
				Init();
			}
			else if(IsMapChanged)
			{
				// The map needs to be repainted
				if(enumerator == null)
				{
					enumerator = PaintMapLayer().GetEnumerator();
				}

				IsMapChanged = enumerator.MoveNext();
			}

			if(_engine != null)
			{
				GetMapUpdatesFromEngine();
			}

			if(_lastInput != MegaDungeon.PlayerInput.NONE)
			{
				_engine.DoTurn(_lastInput);
				_lastInput = MegaDungeon.PlayerInput.NONE;
			}
		}

		public void AcceptInput(Inv.Keystroke keystroke)
		{
			if(keyMap.ContainsKey(keystroke.Key))
			{
				_lastInput = keyMap[keystroke.Key];
			}
		}

		public void Graphics()
		{
			var cloth = new Cloth();
			cloth.Dimension = new Inv.Dimension(_horizontalCellCount, _verticalCellCount); // 35 x 40
			// cloth.CellSize = _surface.Window.Width / _horizontalCellCount;

			cloth.DrawEvent += (DC, Patch) =>
			{
				var glyph = _engine.Floor[Patch.X, Patch.Y];
				var bytes = _tileManager.GetTileBmpBytes(glyph);
				var image = _tileManager.GetInvImage(glyph);
				DC.DrawImage(image, Patch.Rect);

				// if (_engine.Floor[Patch.X, Patch.Y] == 0)
				// 	DC.DrawRectangle(Inv.Colour.DarkSlateGray, null, 0, Patch.Rect);
				// else
				// 	DC.DrawRectangle(Inv.Colour.LightGray, null, 0, Patch.Rect);
			};
			_surface.Content = cloth;
			cloth.Draw();
		}
		
		IEnumerable<Label> CreateCells()
		{
			var h = _root_stack.Window.Height / _verticalCellCount;
			var w = _root_stack.Window.Width / _horizontalCellCount;
			var start = DateTime.UtcNow;
			// var colorStep = 360.0 / (hcells * wcells);
			// var hue = 0.0;
			for (int i = 0; i < _verticalCellCount; i++)
			{
				var hstack = _surface.NewStack(Orientation.Horizontal);
				hstack.Size.SetHeight(h);
				hstack.Size.SetWidth(w * _horizontalCellCount);
				_root_stack.AddPanel(hstack);
				for (int j = 0; j < _horizontalCellCount; j++)
				{
					var label = _surface.NewLabel();
					label.Size.Set(w, h);
					label.Background.Colour = Colour.Black;
					label.Font.Colour = Colour.White;
					label.Justify.Center();
					_labels[j, i] = label;
					hstack.AddPanel(label);
					yield return label;
				}
			}
			var end = DateTime.UtcNow;
			var ms = (end-start).TotalMilliseconds;
			Console.WriteLine($"Created in {ms} milliseconds.");
		}

		/// <summary>
		/// Iterating the enumerator paints a single cell.
		/// </summary>
		/// <remarks>
		/// The caller can finely control how many cells to update in each call by
		/// getting the enumerator and calling GetNext();
		/// </remarks>
		/// <returns></returns>
		public IEnumerable<Label> PaintMapLayer()
		{
			var clStart = DateTime.UtcNow;

			var clsEnd = DateTime.UtcNow;
			var ms = (clsEnd - clStart).TotalMilliseconds;
			Console.WriteLine($"Engine init took {ms} milliseconds");
			for(int x = 0; x < _horizontalCellCount; x++)
			{
				for(int y = 0; y < _verticalCellCount; y++)
				{
					if(_engine.Floor[x,y] == 1)
					{
						_labels[x,y].Background.Colour = Colour.GraySmoke;
					}
					_surface.Window.Sleep(TimeSpan.FromTicks(1));
					yield return _labels[x,y];
				}
			}
			var end = DateTime.UtcNow;
			 ms = (end-clStart).TotalMilliseconds;
			Console.WriteLine($"Colored in {ms} milliseconds.");
		}

		void Init()
		{
				if(enumerator == null)
				{
					enumerator = CreateCells().GetEnumerator();
				}
				IsInit = !enumerator.MoveNext();
				if(IsInit)
				{
					enumerator = null;
				}
		}

			void GetMapUpdatesFromEngine()
			{
				foreach(var actor in _engine.EntityManager.GetAllEntitiesWithComponent<LocationComponent>())
				{

					var location = actor.GetComponent<LocationComponent>();
					var glyph = actor.GetComponent<GlyphComponent>();

					if(!_lastLocation.ContainsKey(actor))
					{
						_lastLocation.Add(actor, location.Clone());
						_labels[location.X, location.Y].Text = glyph.glyph;
					}
					var last = _lastLocation[actor];
					if(last != location)
					{
						_labels[last.X, last.Y].Text = "";
						_labels[location.X, location.Y].Text = glyph.glyph;
					}

					last.X = location.X;
					last.Y = location.Y;
				}
			}
	}
}
