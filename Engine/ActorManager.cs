using EntityComponentSystemCSharp;
using EntityComponentSystemCSharp.Components;
using static MegaDungeon.EngineConstants;

namespace MegaDungeon
{
	public class ActorManager
	{
		EntityManager _entityManager;
		public ActorManager(EntityManager entityManager)
		{
			_entityManager = entityManager;
		}

		public EntityManager.Entity GetPlayerActor()
		{
			var actor = CreateActor(
				PLAYER,
				"Rogue",
				100,
				10,
				10,
				100,
				10
			);
			actor.AddComponent(new SightStat(){Range=4});
			actor.AddComponent<Player>();
			return actor;
		}

		public EntityManager.Entity CreateActor(int glyph, string name = "actor", int maxHealth = 1,int defence = 1, int power = 1,int accuracy = 50, float speed = 6.6F)
		{
			var actor = _entityManager.CreateEntity();
			actor.AddComponent(new Actor(){Gold = 0, Speed  = speed});
			actor.AddComponent(new AttackStat(){Accuracy = accuracy, Power = power});
			actor.AddComponent(new DefenseStat(){Chance = defence});
			actor.AddComponent(new Life(){Health = maxHealth, MaxHealth = maxHealth});
			actor.AddComponent(new Name(){NameString = name});
			actor.AddComponent(new Glyph{glyph = glyph});
			return actor;
		}
	}

}
