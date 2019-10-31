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
		public SystemBase(IEngine engine)
		{
			_em = engine.GetEntityManager();
			_logger = engine.GetLogger();
			_map = engine.GetMap();
		}
		public abstract void Run(Entity entity);
	}
}