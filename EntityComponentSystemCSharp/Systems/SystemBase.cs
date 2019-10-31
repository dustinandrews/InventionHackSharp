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
		protected IEngine _engine;
		public SystemBase(IEngine engine)
		{
			_em = engine.GetEntityManager();
			_logger = engine.GetLogger();
			_map = engine.GetMap();
			_engine = engine;
		}
		public abstract void Run(Entity entity);
	}
}