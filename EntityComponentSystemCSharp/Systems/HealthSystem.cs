using System;
using EntityComponentSystemCSharp.Components;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
	public class HealthSystem : SystemBase, ISystem
	{
		int CORPSE = 636;
		Random rand = new Random();
		public HealthSystem(EntityManager em, ISystemLogger logger, IMap map) : base(em, logger, map)
		{
		}

		public override void Run(EntityManager.Entity entity)
		{
			var life = entity.GetComponent<Life>();
			if(life == null) {return;}

			if(life.Health < life.MaxHealth)
			{
				var regenRole = rand.Next(1000);
				if (regenRole < life.MaxHealth)
				{
					life.Health++;
				}
			}

			if(life.Health <= 0)
			{
				var nameComponent = entity.GetComponent<Name>();
				string name = "";
				if(nameComponent != null)
				{
					name = nameComponent.NameString;
				}

				_logger.Log($"{name}({entity.Id.ToString()}) has run out of hitpoints and died.");

				entity.RemoveComponent<Actor>();
				entity.RemoveComponent<Life>();
				entity.AddComponent<Dead>();

				var glyph = entity.GetComponent<Glyph>();
				glyph.glyph = CORPSE;
			}
		}
	}
}