using System;
using System.Collections.Generic;
using CommerceSim.Components;
using CommerceSim.Systems;
using static CommerceSim.EntityManager;

namespace CommerceSim
{
	class Example
	{
		static string[] items = {"food", "goods", "luxuries"};
		static int turns = 2;
		static int baseCount = 3;
		static void Run(string[] args)
		{
			var em = new EntityManager();

			var entities = CreateEntities(em);
			var demandSystem = new DemandSystem(em);
			var productionSystem = new ProductionSystem(em);

			productionSystem.Run();
			PrintInventories(entities);
			Console.WriteLine();
			for(int i = 0; i < turns; i++)
			{
				Console.WriteLine($"run {i}");
				productionSystem.Run();
				PrintInventories(entities);
				demandSystem.Run();
				PrintInventories(entities);
				Console.WriteLine();
			}
		}

		static void PrintInventories(IEnumerable<Entity> entities)
		{
			foreach(var baseEntity in entities)
			{
				Console.WriteLine(baseEntity.Id);
				var counts = new Dictionary<string, int>();
				var inventory = baseEntity.GetComponent<InventoryComponent>();
				foreach(var itemEntity in inventory.Items)
				{
					var item = itemEntity.GetComponent<ItemComponent>();
					var itemType = item.Type;
					Console.WriteLine($" {itemType}");
				}
			}
		}

		static DemandComponent CreateDemand()
		{
			var demand = new DemandComponent();
			foreach(var item in items)
			{
				demand.Demands.Add(item, 1);
			}
			return demand;
		}

		static Entity[] CreateEntities(EntityManager em)
		{
			var demand = CreateDemand();
			var entities = new List<Entity>();
			for(int i = 0; i < baseCount; i++)
			{
				var entity = em.CreateEntity();
				entities.Add(entity);
				entity.AddComponent(demand);
				var inventory = new InventoryComponent();
				entity.AddComponent(inventory);

				var producer = new ProducerComponent();
				foreach(var item in items)
				{
					var itemProduction = new ProducerComponent.ProductionItem();
					itemProduction.product = item;
					itemProduction.rate = 2.5F;
					producer.ProducedItems.Add(itemProduction);
				}
				entity.AddComponent(producer);
			}


			return entities.ToArray();
		}

		static ProducerComponent CreateWidgetFactory(string itemType)
		{
			var producer = new ProducerComponent();
			var item = new ProducerComponent.ProductionItem(){product = itemType, rate = 1.5F};
			producer.ProducedItems.Add(item);
			return producer;
		}
	}
}
