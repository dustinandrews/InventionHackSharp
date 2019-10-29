using System;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
	public class HealthSystem : SystemBase, ISystem
	{
		public HealthSystem(EntityManager em, ISystemLogger logger, IMap map) : base(em, logger, map)
		{
		}

		public override void Run()
		{
			throw new NotImplementedException();
		}
	}
}