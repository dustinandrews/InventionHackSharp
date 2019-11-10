using System.Collections.Generic;
using RogueSharp;

namespace MegaDungeon
{
	/// <summary>
	/// A collection of constants used by the Engine class.
	/// </summary>
	public class EngineConstants
	{
		public static Point UP = new Point(0,-1);
		public static Point UPRIGHT = new Point(1, -1);
		public static Point RIGHT = new Point(1, 0);
		public static Point DOWNRIGHT = new Point(1, 1);
		public static Point DOWN = new Point(0, 1);
		public static Point DOWNLEFT = new Point(-1, 1);
		public static Point LEFT = new Point(-1, 0);
		public static Point UPLEFT = new Point(-1, -1);
		public static Point CENTER = new Point(0,0);

		public const int FLOOR = 848;
		public const int TILEABLEWALL = 1058;
		public const int DARK = 829;
		public const int DIRT = 1013;
		public const int DOOR = 844;
		public const int PLAYER = 348;

		public static Point[] COMPASSPOINTS = new[] {
				UPLEFT, UP, UPRIGHT,
				LEFT, CENTER, RIGHT,
				DOWNLEFT, DOWN, DOWNRIGHT
			};

		public static Dictionary<PlayerInput, Point> INPUTMAP = new Dictionary<PlayerInput, Point>()
			{
				{PlayerInput.UP, UP},
				{PlayerInput.UPRIGHT, UPRIGHT},
				{PlayerInput.RIGHT, RIGHT},
				{PlayerInput.DOWNRIGHT, DOWNRIGHT},
				{PlayerInput.DOWN, DOWN},
				{PlayerInput.DOWNLEFT, DOWNLEFT},
				{PlayerInput.LEFT, LEFT},
				{PlayerInput.UPLEFT, UPLEFT},
				{PlayerInput.WAIT, CENTER},
			};
	}
}
