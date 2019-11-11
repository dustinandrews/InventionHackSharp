using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using NUnit.Framework;
using EntityComponentSystemCSharp;
using EntityComponentSystemCSharp.Components;
using System.Collections.Generic;
using Newtonsoft.Json;
using static EntityComponentSystemCSharp.EntityManager;
using MegaDungeon.Contracts;

namespace EntityComponentSystemCSharp.Test
{
	[TestFixture]
	public class EntityPrototypeTests
	{
		[Test]
		public void GetEntityDetailsTest()
		{
			var em = new EntityManager();
			var entity = em.CreateEntity();
			entity.AddComponent(new Location(){X = 1, Y = 2});
			entity.AddComponent<Actor>();
			var prototype = em.GetEntityPrototype(entity);
			Assert.NotNull(prototype["Actor"]);
		}

		[Test]
		public void RoundTripTest()
		{
			var em = new EntityManager();
			var entity = em.CreateEntity();
			entity.AddComponent(new Location(){X = 1, Y = 2});
			entity.AddComponent<Actor>();
			var prototype = em.GetEntityPrototype(entity);
			var protoString = JsonConvert.SerializeObject(prototype);
			var newEntity = em.NewEntityFromPrototype(protoString);
			var location = newEntity.GetComponent<Location>();
			Assert.AreEqual(1, location.X);
			Assert.AreEqual(2, location.Y);
			Assert.AreNotEqual(entity.Id, newEntity.Id);
		}
		
		[Test]
		public void CreateRealEntityPrototype()
		{
			var em = new EntityManager();
			var actor1 = CreateActor(em, 0, "Giant Ant", 10, 5, 2, 50, 9.5F);
			var actor2 = CreateActor(em, 71, "Goblin", 5, 1, 2, 50);
			var actor3 = CreateActor(em, 60, "Kobold");
			var serial1 = em.GetEntityPrototype(actor1);
			var serial2 = em.GetEntityPrototype(actor2);
			var serial3 = em.GetEntityPrototype(actor3);
			var dict = new Dictionary<string, Dictionary<string, IComponent>>(){ 
				{"Giant Ant", serial1},
				{"Goblin", serial2},
				{"Kobold", serial3}};
			var json = JsonConvert.SerializeObject(dict, Formatting.Indented);
			var codebase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
			var testPath = (new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
			var protoTypePath = Path.Combine(Path.GetDirectoryName(testPath), "EntityPrototypes.json");
			System.IO.File.WriteAllText(protoTypePath, json);
		}

		EntityManager.Entity CreateActor(EntityManager em, int glyph, string name = "actor", int maxHealth = 1,int defense = 1, int power = 1,int accuracy = 70, float speed = 8.0F)
		{
			var actor = em.CreateEntity();
			actor.AddComponent(new Actor(){Gold = 0, Speed  = speed});
			actor.AddComponent(new AttackStat(){Accuracy = accuracy, Power = power});
			actor.AddComponent(new DefenseStat(){Chance = defense});
			actor.AddComponent(new Life(){Health = maxHealth, MaxHealth = maxHealth});
			actor.AddComponent(new Name(){NameString = name});
			actor.AddComponent(new Glyph{glyph = glyph});

			actor.AddComponent(new Faction(){Type = Factions.Monster});
			actor.AddComponent<WanderingMonster>();
			return actor;
		}
	}
}