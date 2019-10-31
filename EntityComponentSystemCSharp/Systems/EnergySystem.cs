using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using EntityComponentSystemCSharp.Components;

namespace EntityComponentSystemCSharp.Systems
{
	public class EnergySystem : SystemBase, ISystem
	{
		public EnergySystem(IEngine engine) : base(engine)
		{
		}

		public override void Run(EntityManager.Entity entity)
		{
			if(!entity.HasComponent<Actor>()){return;}
			
			var actor = entity.GetComponent<Actor>();
			actor.Energy += actor.Speed;
			
		}
	}
}