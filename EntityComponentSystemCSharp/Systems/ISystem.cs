namespace EntityComponentSystemCSharp.Systems
{
	public interface ISystem
	{
		void Run();
	}

	public interface ISystemLogger
	{
		void Log(string message);
	}
}