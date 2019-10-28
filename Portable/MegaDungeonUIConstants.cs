using System.Collections.Generic;
using Inv;

namespace Portable
{
	public static class MegaDungeonUIConstants
	{
		/// <summary>
		/// Mapping from UI implementation to game.
		/// </summary>
		/// <remarks>
		/// Enforce a strong firewall between the game the UI implmentation.
		/// </remarks>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="MD.PlayerInput"></typeparam>
		/// <returns></returns>
		public static Dictionary<Inv.Key, MegaDungeon.PlayerInput> KeyMap = new Dictionary<Key, MegaDungeon.PlayerInput>()
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

			// vi keys
			{Inv.Key.B,MegaDungeon.PlayerInput.DOWNLEFT},
			{Inv.Key.J,MegaDungeon.PlayerInput.DOWN},
			{Inv.Key.N,MegaDungeon.PlayerInput.DOWNRIGHT},
			{Inv.Key.H,MegaDungeon.PlayerInput.LEFT},
			{Inv.Key.L,MegaDungeon.PlayerInput.RIGHT},
			{Inv.Key.Y,MegaDungeon.PlayerInput.UPLEFT},
			{Inv.Key.K,MegaDungeon.PlayerInput.UP},
			{Inv.Key.U,MegaDungeon.PlayerInput.UPRIGHT},

			// QWEASDZC
			{Inv.Key.Z,MegaDungeon.PlayerInput.DOWNLEFT},
			{Inv.Key.S,MegaDungeon.PlayerInput.DOWN},
			{Inv.Key.C,MegaDungeon.PlayerInput.DOWNRIGHT},
			{Inv.Key.A,MegaDungeon.PlayerInput.LEFT},
			{Inv.Key.D,MegaDungeon.PlayerInput.RIGHT},
			{Inv.Key.Q,MegaDungeon.PlayerInput.UPLEFT},
			{Inv.Key.W,MegaDungeon.PlayerInput.UP},
			{Inv.Key.E,MegaDungeon.PlayerInput.UPRIGHT},
		};
	}
}
