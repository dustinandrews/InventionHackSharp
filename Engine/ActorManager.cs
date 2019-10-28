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
				2,
				50,
				10
			);
			actor.AddComponent(new Sight(){Range=4});
			actor.AddComponent<Player>();
			return actor;
		}

		public EntityManager.Entity CreateActor(int glyph, string name = "actor", int maxHealth = 1,int defence = 1, int power = 1,int accuracy = 50,int speed = 10)
		{
			var actor = _entityManager.CreateEntity();
			actor.AddComponent(new Actor(){Gold = 0, Speed  = speed});
			actor.AddComponent(new Attack(){Accuracy = accuracy, Power = power});
			actor.AddComponent(new Defense(){Chance = defence});
			actor.AddComponent(new Alive(){Health = maxHealth, MaxHealth = maxHealth});
			actor.AddComponent(new Name(){NameString = name});
			actor.AddComponent(new Glyph{glyph = glyph});
			return actor;
		}
	}

}
