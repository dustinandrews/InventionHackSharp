using System;
using System.Linq;
using System.Collections.Generic;
using EntityComponentSystemCSharp.Components;
using EntityComponentSystemCSharp;
using static EntityComponentSystemCSharp.EntityManager;
using static Portable.MegaDungeonUIConstants;
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
		Dock _outerDock;
		Dock _innerDock;
		Entity _player;

		public MegaDungeon.PlayerInput LastInput { get => _lastInput; set => _lastInput = value; }

		/// <summary>
		/// Constructor for Main UI logic class. Sets up all UI windows.
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="horizontalCellCount"></param>
		/// <param name="vericalCellCount"></param>
		public MegaDungeonUI(Surface surface, int horizontalCellCount, int vericalCellCount)
		{
			_tileManager = new TileManager("absurd64.bmp", System.IO.File.ReadAllText("tiledata.json"));
			_horizontalCellCount = horizontalCellCount;
			_verticalCellCount = vericalCellCount;
			_surface = surface;
			_cloth = CreateCloth();
			_outerDock = surface.NewDock(Orientation.Vertical);
			_innerDock = surface.NewDock(Orientation.Horizontal);
			
			var l1 = surface.NewLabel();
			l1.Text = "outer header";
			l1.Font.Colour = Colour.White;
			l1.Border.Set(1);
			l1.Border.Colour = Colour.Tomato;
			var l2 = surface.NewLabel();
			l2.Text = "outer footer";
			l2.Font.Colour = Colour.White;
			l2.Border.Set(1);
			l2.Border.Colour = Colour.Tomato;

			var lc = surface.NewLabel();
			lc.Text = "client";
			lc.Font.Colour = Colour.White;
			lc.Border.Set(1);
			lc.Border.Colour = Colour.Magenta;

			_outerDock.AddHeader(l1);
			_outerDock.AddClient(_innerDock);
			_outerDock.AddFooter(l2);

			var l3 = surface.NewLabel();
			l3.Text = "inner header";
			l3.Font.Colour = Colour.White;
			l3.Border.Colour = Colour.Tomato;
			l3.Border.Set(1);
			var l4 = surface.NewLabel();
			l4.Text = "inner footer";
			l4.Font.Colour = Colour.White;
			l4.Border.Colour = Colour.Tomato;
			l4.Border.Set(1);
			_innerDock.AddHeader(l3);
			_innerDock.AddClient(_cloth);
			_innerDock.AddFooter(l4);


			_surface.Content = _outerDock;
			_cloth.Draw();
			_surface.ComposeEvent += () =>
			{
				Update();
			};
			
			_engine = new MegaDungeon.Engine(_horizontalCellCount, _verticalCellCount, _tileManager);
			_player = _engine.EntityManager.GetAllEntitiesWithComponent<PlayerComponent>().First();
			GetActorsFromEngine();
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
			if (_lastInput != MegaDungeon.PlayerInput.NONE)
			{
				_engine.DoTurn(_lastInput);
				_lastInput = MegaDungeon.PlayerInput.NONE;
				GetActorsFromEngine();
				var playerLocation = _lastLocation[_player];
				_cloth.KeepXYOnScreen(playerLocation.X, playerLocation.Y);
				_cloth.Draw();
			}
		}

		public void AcceptInput(Inv.Keystroke keystroke)
		{
			if (KeyMap.ContainsKey(keystroke.Key))
			{
				_lastInput = KeyMap[keystroke.Key];
			}
		}

		internal Cloth CreateCloth()
		{
			var cloth = new Cloth();
			cloth.Dimension = new Inv.Dimension(_horizontalCellCount, _verticalCellCount);
			cloth.CellSize = _surface.Window.Width / _horizontalCellCount;
			cloth.DrawEvent += (DC, patch) => Cloth_DrawEvent(DC, patch);
			return cloth;
		}

		void Cloth_DrawEvent(DrawContract DC, Patch patch)
		{
			int glyph = _engine.Floor[patch.X, patch.Y];
			var image = _tileManager.GetInvImage(glyph);
			DC.DrawImage(image, patch.Rect);

			if(_actorLocationMap.ContainsKey(patch.X + (patch.Y * _horizontalCellCount)))
			{
				glyph = _actorLocationMap[patch.X + (patch.Y * _horizontalCellCount)];
				var actorImage = _tileManager.GetInvImage(glyph);
				DC.DrawImage(actorImage, patch.Rect);
			}
		}

		void GetActorsFromEngine()
		{
			foreach (var actor in _engine.EntityManager.GetAllEntitiesWithComponent<LocationComponent>())
			{

				var location = actor.GetComponent<LocationComponent>();
				var glyph = actor.GetComponent<GlyphComponent>();

				if (!_lastLocation.ContainsKey(actor))
				{
					_lastLocation.Add(actor, location.Clone());
					_actorLocationMap.Add(location.X + (location.Y * _horizontalCellCount), glyph.glyph);
				}

				var last = _lastLocation[actor];
				if (last != location)
				{
					_actorLocationMap.Remove(last.X + (last.Y * _horizontalCellCount));
					_actorLocationMap.Add(location.X + (location.Y * _horizontalCellCount), glyph.glyph);
				}

				last.X = location.X;
				last.Y = location.Y;
			}
		}
	}
}
