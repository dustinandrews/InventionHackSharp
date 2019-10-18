using System;
using System.Linq;
using System.Collections.Generic;
using EntityComponentSystemCSharp.Components;
using static EntityComponentSystemCSharp.EntityManager;
using Inv;
using MD;

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
		MD.Engine _engine;

		MD.PlayerInput _lastInput = MD.PlayerInput.NONE;

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
		public Dictionary<Inv.Key, MD.PlayerInput> keyMap = new Dictionary<Key, MD.PlayerInput>()
		{
			{Inv.Key.n1, MD.PlayerInput.DOWNLEFT},
			{Inv.Key.n2, MD.PlayerInput.DOWN},
			{Inv.Key.n3, MD.PlayerInput.DOWNRIGHT},
			{Inv.Key.n4, MD.PlayerInput.LEFT},
			{Inv.Key.n5, MD.PlayerInput.NONE},
			{Inv.Key.n6, MD.PlayerInput.RIGHT},
			{Inv.Key.n7, MD.PlayerInput.UPLEFT},
			{Inv.Key.n8, MD.PlayerInput.UP},
			{Inv.Key.n9, MD.PlayerInput.UPRIGHT},
			{Inv.Key.Up, MD.PlayerInput.UP},
			{Inv.Key.Down, MD.PlayerInput.DOWN},
			{Inv.Key.Left, MD.PlayerInput.LEFT},
			{Inv.Key.Right, MD.PlayerInput.RIGHT},
		};

			public PlayerInput LastInput { get => _lastInput; set => _lastInput = value; }

		public MegaDungeonUI(Surface surface, int horizontalCellCount, int vericalCellCount)
		{
			_horizontalCellCount = horizontalCellCount;
			_verticalCellCount = vericalCellCount;
			_surface = surface;
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

			if(_lastInput != MD.PlayerInput.NONE)
			{
				_engine.DoTurn(_lastInput);
				_lastInput = MD.PlayerInput.NONE;
			}
		}

		public void AcceptInput(Inv.Keystroke keystroke)
		{
			if(keyMap.ContainsKey(keystroke.Key))
			{
				_lastInput = keyMap[keystroke.Key];
			}
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
					_labels[j, i] = label;
					hstack.AddPanel(label);
					// label.Text = $"{i},{j}";
					// label.Font.Size = 9;
					// label.Border.Colour = Colour.Yellow;
					// label.Border.Set(1);
					// label.Background.Colour = Colour.FromHSV(hue, 0.5, 0.5);
					// hue += colorStep;
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
			_engine = new MD.Engine(_horizontalCellCount,_verticalCellCount);
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
					var location = _engine.EntityManager.GetComponent<LocationComponent>(actor);
					if(!_lastLocation.ContainsKey(actor))
					{
						_lastLocation.Add(actor, location.Clone());
						_labels[location.X, location.Y].Background.Colour = Colour.Yellow;
					}
					var last = _lastLocation[actor];
					if(last != location)
					{
						_labels[last.X, last.Y].Background.Colour = Colour.Black;
						_labels[location.X, location.Y].Background.Colour = Colour.Yellow;
					}
					last.X = location.X;
					last.Y = location.Y;
				}
			}
	}
}
