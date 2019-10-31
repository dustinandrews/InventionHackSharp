using System;
using EntityComponentSystemCSharp.Components;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{

    public class CombatSystem : SystemBase, ISystem
	{
		Random _rand = new Random();

		public CombatSystem(EntityManager em, ISystemLogger logger, IMap map) : base(em, logger, map)
		{
		}

		public override void Run(EntityManager.Entity entity)
		{
			if(!entity.HasComponent<Attacked>()) {return;}
			
			var attacked = entity.GetComponent<Attacked>();
			var alive = entity.GetComponent<Life>();
			var attackStat = attacked.attacker.GetComponent<AttackStat>();
			var defense = entity.GetComponent<DefenseStat>();
			if(alive != null && attackStat != null)
			{
				var hit = _rand.Next(100);
				if(hit < attackStat.Accuracy)
				{
					var damage = _rand.Next(attackStat.Power) + 1;
					if(defense != null)
					{
						if (hit > defense.Chance)
						{
							_logger.Log($"Hit {damage}.");
							alive.Health -= damage;
						}
					}
				}
				else
				{
					_logger.Log("miss!");
				}
			}
			entity.RemoveComponent<Attacked>();
		}
	}
}