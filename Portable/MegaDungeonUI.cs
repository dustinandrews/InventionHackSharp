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
		Label _topLabel;
		Label _bottomLabel;
		Label _rightLabel;
		Label _leftLabel;

		public MegaDungeon.PlayerInput LastInput { get => _lastInput; set => _lastInput = value; }

		/// <summary>
		/// Constructor for Main UI logic class. Sets up all UI windows.
		/// </summary>
		/// <param name="surface"></param>
		/// <param name="horizontalCellCount"></param>
		/// <param name="verticalCellCount"></param>
		public MegaDungeonUI(Surface surface, int horizontalCellCount, int verticalCellCount)
		{
			_horizontalCellCount = horizontalCellCount;
			_verticalCellCount = verticalCellCount;

			_tileManager = new TileManager("absurd64.bmp", System.IO.File.ReadAllText("tiledata.json"));
			_engine = new MegaDungeon.Engine(horizontalCellCount, verticalCellCount, _tileManager);

			_surface = surface;
			_cloth = CreateCloth();
			_outerDock = _surface.NewDock(Orientation.Vertical);
			_innerDock = _surface.NewDock(Orientation.Horizontal);
			
			_topLabel = InitLabel("Top", ColorPallette.PrimaryColorLightest, ColorPallette.PrimaryColorDarkest, ColorPallette.PrimaryColorDarker);
			_bottomLabel = InitLabel("Bottom", ColorPallette.Secondary1ColorLightest, ColorPallette.Secondary1ColorDarkest, ColorPallette.Secondary1ColorDarker);
			_outerDock.AddHeader(_topLabel);
			_outerDock.AddClient(_innerDock);
			_outerDock.AddFooter(_bottomLabel);

			_leftLabel = InitLabel("Left", ColorPallette.Secondary2ColorLightest, ColorPallette.Secondary2ColorDarkest, ColorPallette.Secondary2ColorDarker);
			_rightLabel = InitLabel("Right", ColorPallette.ComplementColorLightest, ColorPallette.ComplementColorDarkest, ColorPallette.ComplementColorDarker);
			_innerDock.AddHeader(_leftLabel);
			_innerDock.AddClient(_cloth);
			_innerDock.AddFooter(_rightLabel);
			
			_surface.Content = _outerDock;
			
			_surface.ComposeEvent += Warmup();
		}


		/// <summary>
		/// Check for full _cloth init before going to usual update
		/// </summary>
		/// <returns>null</returns>
		/// <remarks>
		/// Update gets called a few times before the window is fully laid out.
		/// When base dimensions are available center the player and connect main Update();
		/// Cannot use a lambda because -= won't be able to unregister without a method handle.
		/// </remarks>
		System.Action Warmup()
		{
			return new Action( () =>
			{
				if(_cloth.BaseDimension.Height == 0)
				{
					return;
				}
				_surface.ComposeEvent -= Warmup();
				_cloth.SetPanningXY(_engine.PlayerLocation.X, _engine.PlayerLocation.Y);
				GetActorsFromEngine();
				_surface.ComposeEvent += () => Update();
				_cloth.Draw();
			});
		}

		/// <summary>
		/// Update one atomic unit of the UI.
		/// </summary>
		/// <remarks>
		/// This method is the pump for the game. It should be fast enough to always return in less than
		/// one frame (1/60th sec.)
		/// </remarks>
		void Update()
		{
			// GetActorsFromEngine();
			// var playerLocation = _lastLocation[_player];
			// _cloth.SetPanningXY(playerLocation.X, playerLocation.Y);
			if (_lastInput != MegaDungeon.PlayerInput.NONE)
			{
				_engine.DoTurn(_lastInput);
				_lastInput = MegaDungeon.PlayerInput.NONE;
				GetActorsFromEngine();
				_cloth.KeepXYOnScreen(_engine.PlayerLocation.X, _engine.PlayerLocation.Y);
				_bottomLabel.Text = string.Join("\n", _engine.Messages);
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

		Label InitLabel(string text, Colour fontColor,  Colour border, Colour background)
		{
			var newLabel = _surface.NewLabel();
			newLabel.Text = text;
			newLabel.Background.Colour = background;
			newLabel.Font.Colour = fontColor;
			newLabel.Border.Colour = border;
			newLabel.Border.Set(1);
			return newLabel;
		}

		internal Cloth CreateCloth()
		{
			var cloth = new Cloth();
			cloth.Dimension = new Inv.Dimension(_horizontalCellCount, _verticalCellCount);
			cloth.CellSize = (_surface.Window.Width / _horizontalCellCount) * 4; //How much of initial map to show.
			cloth.Draw();
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
					_lastLocation.Add(actor, new LocationComponent(){X = location.X, Y = location.Y});
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
