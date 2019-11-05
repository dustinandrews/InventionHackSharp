using EntityComponentSystemCSharp.Systems;

namespace MegaDungeon
{
    public class EngineLogger : ISystemLogger
	{
		Engine _engine;
		public EngineLogger(Engine engine)
		{
			_engine = engine;
		}

		public void Log(string message)
		{
			_engine._messages.Enqueue(message);
		}
	}
}
