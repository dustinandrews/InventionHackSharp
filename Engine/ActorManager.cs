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

		public EntityManager.Entity GetPlayerActor(int playerGlyph)
		{
			var actor = CreateActor(
				playerGlyph,
				name: "Valkerie",
				maxHealth: 10,
				defense: 10,
				power: 10,
				accuracy: 75,
				speed: 10
			);
			actor.AddComponent(new SightStat(){Range=5});
			actor.AddComponent<Player>();
			return actor;
		}

		public EntityManager.Entity CreateActor(int glyph, string name = "actor", int maxHealth = 1,int defense = 1, int power = 1,int accuracy = 70, float speed = 8.0F)
		{
			var actor = _entityManager.CreateEntity();
			actor.AddComponent(new Actor(){Gold = 0, Speed  = speed});
			actor.AddComponent(new AttackStat(){Accuracy = accuracy, Power = power});
			actor.AddComponent(new DefenseStat(){Chance = defense});
			actor.AddComponent(new Life(){Health = maxHealth, MaxHealth = maxHealth});
			actor.AddComponent(new Name(){NameString = name});
			actor.AddComponent(new Glyph{glyph = glyph});
			return actor;
		}
	}

}
