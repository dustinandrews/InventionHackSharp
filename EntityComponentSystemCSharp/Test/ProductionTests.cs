using System;
using NUnit.Framework;
using CommerceSim.Components;
using CommerceSim.Systems;
using System.Linq;

namespace CommerceSim
{
	public class ProductionTests
	{
		[Test]
		public void ConstructorTest()
		{
			var productionSystem = new ProductionSystem(new EntityManager());
		}

		[Test]
		public void ProductionTest()
		{
			var em = new EntityManager();
			var entity = em.CreateEntity();
			var producer =  CreateWidgetFactory();
			entity.AddComponent( producer);

			var inventory = new InventoryComponent();
			entity.AddComponent( inventory);

			var productionSystem = new ProductionSystem(em);
			productionSystem.Run();

			Assert.AreEqual(11, inventory.Items.Count);
		}


		[Test]
		public void InventoryLimitTest()
		{
			var em = new EntityManager();
			var entity = em.CreateEntity();
			var producer =  CreateWidgetFactory();
			entity.AddComponent(producer);

			var inventory = new InventoryComponent();
			inventory.Size = 10;
			entity.AddComponent( inventory);

			var productionSystem = new ProductionSystem(em);
			productionSystem.Run();
			Assert.AreEqual(10, inventory.Items.Count);
		}

		[Test]
		public void ProducedItemNameTest()
		{
			var em = new EntityManager();
			var entity = em.CreateEntity();
			var producer =  CreateWidgetFactory();
			entity.AddComponent( producer);
			var inventory = new InventoryComponent();
			inventory.Size = 10;
			entity.AddComponent( inventory);
			var productionSystem = new ProductionSystem(em);
			productionSystem.Run();

			var item = inventory.Items[0];
			var name = item.GetComponent<ItemComponent>().Type;
			Assert.AreEqual(name, producer.ProducedItems[0].product);
		}

		[Test]
		public void DemandTest()
		{
			var em = new EntityManager();
			var entity = em.CreateEntity();
			var producer =  CreateWidgetFactory();
			entity.AddComponent(producer);
			var inventory = new InventoryComponent();
			inventory.Size = 10;
			entity.AddComponent( inventory);
			var productionSystem = new ProductionSystem(em);
			productionSystem.Run();

			var demand = new DemandComponent();
			demand.Demands.Add("widget", 1);
			entity.AddComponent( demand);
			var demandSystem = new DemandSystem(em);
			demandSystem.Run();

			Assert.Less(inventory.Items.Count, 10);
		}

		[Test]
		public void StackSystemTest()
		{
			var em = new EntityManager();
			var entity = em.CreateEntity();
			var inventory = new InventoryComponent();
			entity.AddComponent(inventory);
			var itemEntity = em.CreateEntity();
			var itemComponent = new ItemComponent(){Type = "widget"};
			itemEntity.AddComponent<StackableComponent>();
			itemEntity.AddComponent(itemComponent);
			inventory.Items.Add(itemEntity);

			itemEntity = em.CreateEntity();
			itemEntity.AddComponent(itemComponent);
			itemEntity.AddComponent<StackableComponent>();
			inventory.Items.Add(itemEntity);

			var stackSystem = new StackSystem(em);
			var stacks = stackSystem.GetStacks(entity);

			Assert.AreEqual(2, stacks["widget"]);
		}

		// * //////////////// Helpers /////////////////
		ProducerComponent CreateWidgetFactory()
		{
			var producer = new ProducerComponent();
			var item = new ProducerComponent.ProductionItem(){product = "widget", rate = 11};
			producer.ProducedItems.Add(item);
			return producer;
		}

	}
}
