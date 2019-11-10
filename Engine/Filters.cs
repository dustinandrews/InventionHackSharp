using System;
using System.Linq;
using RogueSharp;
using FeatureDetector;
using static MegaDungeon.EngineConstants;

namespace MegaDungeon
{
	/// <summary>
	/// Filters for matching small segments of the dungeon against to detect features.
	/// A simplified form of convalutional filters.
	/// </summary>
	public static class Filters
	{
		// 1 == impassable
		public static int[] Vertical = new []{
			0,1,0,
			0,0,0,
			0,1,0
		};

		public static int[,] IdentityFilter = new int[,]{
			{0,0,0},
			{0,1,0},
			{0,0,0}
		};





	}
}
