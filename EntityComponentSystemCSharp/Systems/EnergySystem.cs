using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using EntityComponentSystemCSharp.Components;

namespace EntityComponentSystemCSharp.Systems
{
	public class EnergySystem : SystemBase, ISystem
	{
		public EnergySystem(EntityManager em, ISystemLogger logger, IMap map) : base(em, logger, map)
		{
		}

		public override void Run()
		{
			var actors = _em.GetAllEntitiesWithComponent<Actor>();
			foreach(var entity in actors)
			{
				var actor = entity.GetComponent<Actor>();
				actor.Energy += actor.Speed;
			}
		}
	}
}