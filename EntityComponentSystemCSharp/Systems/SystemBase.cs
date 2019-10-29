using EntityComponentSystemCSharp;
using static EntityComponentSystemCSharp.EntityManager;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
    public abstract class SystemBase: ISystem
	{
		protected readonly EntityManager _em;
		protected readonly ISystemLogger _logger;
		protected readonly IMap _map;
		public SystemBase(EntityManager em, ISystemLogger logger, IMap map)
		{
			_em = em;
			_logger = logger;
			_map = map;
		}
		public abstract void Run();
	}
}