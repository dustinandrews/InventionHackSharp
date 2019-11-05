namespace EntityComponentSystemCSharp.Systems
{
	public interface ISystem
	{
		void Run(EntityManager.Entity entity);
	}

	public interface ISystemLogger
	{
		void Log(string message);
	}
}