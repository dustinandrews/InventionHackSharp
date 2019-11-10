using System.Collections.Generic;
using EntityComponentSystemCSharp.Systems;
using MegaDungeon.Contracts;
using RogueSharp;

namespace EntityComponentSystemCSharp
{
    public interface IEngine
	{
		EntityManager GetEntityManager();
		RogueSharp.IMap GetMap();
		ISystemLogger GetLogger();
		HashSet<Point> GetPlayerViewable();
		Point GetPlayerLocation();
		ITileManager GetTileManager();
	}
}