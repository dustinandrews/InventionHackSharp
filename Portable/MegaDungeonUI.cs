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
		Dictionary<Entity, EntityComponentSystemCSharp.Components.Location> _lastLocation = new Dictionary<Entity, EntityComponentSystemCSharp.Components.Location>();
		Dictionary<int, int> _actorLocationMap = new Dictionary<int, int>();
		Cloth _cloth;
		Dock _outerDock;
		Dock _innerDock;
		Label _topLabel;
		Label _bottomLabel;
		Label _rightLabel;
		Label _gameOver;
		EntityData _leftPanel;
		bool _showDebugInfo = false;

		static Dictionary<Inv.Key, Action> UICommands = new Dictionary<Key, Action>();
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

			var directory = surface.Window.Application.Directory;

			_tileManager = new TileManager(directory.NewAsset("absurd64.bmp"), directory.NewAsset("tiledata.json"));
			_engine = new MegaDungeon.Engine(horizontalCellCount, verticalCellCount, _tileManager);

			_surface = surface;
			_cloth = CreateCloth();
			_outerDock = _surface.NewDock(Inv.Orientation.Vertical);
			_innerDock = _surface.NewDock(Inv.Orientation.Horizontal);
			
			_topLabel = InitLabel("Top", ColorPallette.PrimaryColorLightest, ColorPallette.PrimaryColorDarkest, ColorPallette.PrimaryColorDarker);
			_bottomLabel = InitLabel("Bottom", ColorPallette.Secondary1ColorLightest, ColorPallette.Secondary1ColorDarkest, ColorPallette.Secondary1ColorDarker);
			_outerDock.AddHeader(_topLabel);
			_outerDock.AddClient(_innerDock);
			_outerDock.AddFooter(_bottomLabel);

			var vstack = _surface.NewVerticalStack();
			vstack.Background.Colour = ColorPallette.Secondary1ColorDarkest;
			vstack.Border.Set(5);
			vstack.Border.Colour = ColorPallette.Secondary1ColorDarkest;

			_leftPanel = new EntityData(_surface, ColorPallette.Secondary1ColorLightest, ColorPallette.Secondary1ColorDarkest);
			vstack.AddPanel(_leftPanel.Table);
			_rightLabel = InitLabel("Right", ColorPallette.ComplementColorLightest, ColorPallette.ComplementColorDarkest, ColorPallette.ComplementColorDarker);
			_innerDock.AddHeader(vstack);
			_innerDock.AddClient(_cloth);
			_innerDock.AddFooter(_rightLabel);
			_surface.Content = _outerDock;
			_surface.ComposeEvent += Warmup();

			_gameOver = _surface.NewLabel();
			_gameOver.Text = "YOU HAVE DIED.";
			_gameOver.Font.ExtraMassive();
			_gameOver.Font.Colour = Colour.Red;
			_gameOver.Background.Colour = Colour.DarkBlue;
			_gameOver.Justify.Center();

			InitUiCommands();
		}

		/// <summary>
		/// Commands the only do something in the UI layer and are not passed into the game engine.
		/// </summary>
		void InitUiCommands()
		{
			UICommands[Inv.Key.Plus] = () => {_cloth.Zoom(0,0,1);};
			UICommands[Inv.Key.Minus] = () => {_cloth.Zoom(0,0,-1);};
			UICommands[Inv.Key.F2] = () => {_showDebugInfo = !_showDebugInfo;_cloth.Draw();};
		}
		/// <summary>
		/// Check for full _cloth init before going to usual update
		/// </summary>
		/// <returns>null</returns>
		/// <remarks>
		/// Update gets called a few times before the window is fully laid out.
		/// When base dimensions are available center the player and connect main Update();
		/// (Cannot use a lambda because -= won't be able to unregister without a method handle.)
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
				_leftPanel.SetData(_engine.PlayerEntity);
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
			if (_lastInput != MegaDungeon.PlayerInput.NONE)
			{
				if(!_engine.PlayerEntity.HasComponent<Actor>())
				{
					_surface.Content = _gameOver;
					return;
				}
				_engine.DoTurn(_lastInput);
				GetActorsFromEngine();
				_cloth.KeepXYOnScreen(_engine.PlayerLocation.X, _engine.PlayerLocation.Y);
				_bottomLabel.Text = string.Join("\n", _engine.Messages);
				_leftPanel.SetData(_engine.PlayerEntity);
				_cloth.Draw();
				_lastInput = MegaDungeon.PlayerInput.NONE;
			}
		}

		public void AcceptInput(Inv.Keystroke keystroke)
		{
			if (KeyMap.ContainsKey(keystroke.Key))
			{
				_lastInput = KeyMap[keystroke.Key];
			}
			else if (UICommands.ContainsKey(keystroke.Key))
			{
				UICommands[keystroke.Key]();
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
			cloth.CellSize = (_surface.Window.Width / _horizontalCellCount) * 2; //How much of initial map to show.
			cloth.Draw();
			cloth.DrawEvent += (DC, patch) => Cloth_DrawEvent(DC, patch);
			return cloth;
		}

		void Cloth_DrawEvent(DrawContract dc, Patch patch)
		{
			var point = new RogueSharp.Point(patch.X, patch.Y);
			int glyph = _engine.Floor[patch.X, patch.Y];

			Inv.Image image;
			if(_engine.Viewable.Contains(point))
			{
				if(_actorLocationMap.ContainsKey(patch.X + (patch.Y * _horizontalCellCount)))
				{	
					glyph = _actorLocationMap[patch.X + (patch.Y * _horizontalCellCount)];
					image = _tileManager.GetInvImage(glyph);
					dc.DrawImage(image, patch.Rect);
					if(_showDebugInfo)
					{
						foreach(var loc in _lastLocation)
						{
							if(loc.Value.X == patch.X && loc.Value.Y == patch.Y)
							{
								DrawText(loc.Key.Id.ToString(), dc, patch);
							}
						}
					}
				}
				else
				{
					image = _tileManager.GetInvImage(glyph);
					dc.DrawImage(image, patch.Rect);
				}
			}
			else
			{
				image = _tileManager.GetInvImageDark(glyph);
				dc.DrawImage(image, patch.Rect);
			}
		}

		void DrawText(string text, DrawContract dc, Patch patch)
		{
			dc.DrawText(text, "Courier", 11,FontWeight.Regular, Colour.YellowGreen, new Point(patch.Rect.Left, patch.Rect.Top), HorizontalPosition.Left, VerticalPosition.Top);
		}
		void GetActorsFromEngine()
		{
			// Remove old location from the map.
			foreach (var actor in _engine.GetEntityManager().GetAllEntitiesWithComponent<EntityComponentSystemCSharp.Components.Location>())
			{
				var location = actor.GetComponent<EntityComponentSystemCSharp.Components.Location>();
				var glyph = actor.GetComponent<Glyph>();
				if (_lastLocation.ContainsKey(actor))
				{
					var last = _lastLocation[actor];
					_actorLocationMap.Remove(last.X + (last.Y * _horizontalCellCount));
				}
			}

			// Add new locations
			foreach(var actor in _engine.GetEntityManager().GetAllEntitiesWithComponent<EntityComponentSystemCSharp.Components.Location>())
			{
				var location = actor.GetComponent<EntityComponentSystemCSharp.Components.Location>();
				var glyph = actor.GetComponent<Glyph>();
				_actorLocationMap[location.X + (location.Y * _horizontalCellCount)] = glyph.glyph;
				_lastLocation[actor] = new EntityComponentSystemCSharp.Components.Location() {X = location.X, Y = location.Y};
			}
		}
	}
}
