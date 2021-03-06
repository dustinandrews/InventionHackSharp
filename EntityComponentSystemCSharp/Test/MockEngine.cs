using System.Collections.Generic;
using EntityComponentSystemCSharp.Systems;
using MegaDungeon.Contracts;
using RogueSharp;

namespace EntityComponentSystemCSharp
{
	public class MockEngine : IEngine
	{
		EntityManager _em;
		ISystemLogger _logger;
		IMap _map;
		public MockEngine(EntityManager entityManager, ISystemLogger systemLogger, IMap map)
		{
			_em = entityManager;
			_logger = systemLogger;
			_map = map;
		}
		public EntityManager GetEntityManager()
		{
			return _em;
		}

		public ISystemLogger GetLogger()
		{
			return _logger;
		}

		public IMap GetMap()
		{
			return _map;
		}

		public Point GetPlayerLocation()
		{
			throw new System.NotImplementedException();
		}

		public HashSet<Point> GetPlayerViewable()
		{
			throw new System.NotImplementedException();
		}

		public ITileManager GetTileManager()
		{
			throw new System.NotImplementedException();
		}
	}
}
