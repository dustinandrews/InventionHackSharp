using EntityComponentSystemCSharp.Systems;


namespace EntityComponentSystemCSharp
{
    public interface IEngine
	{
		EntityManager GetEntityManager();
		RogueSharp.IMap GetMap();
		ISystemLogger GetLogger();
	}
}