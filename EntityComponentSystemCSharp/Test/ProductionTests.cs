using NUnit.Framework;
using EntityComponentSystemCSharp.Components;
using EntityComponentSystemCSharp.Systems;
using System.Linq;

namespace EntityComponentSystemCSharp
{
	public partial class ProductionSystemTests
	{
		[Test]
		public void ConstructorTest()
		{
			var productionSystem = new ProductionSystem(new EntityManager(), new MockLogger(), new MockMap());
		}

		[Test]
		public void ProductionTest()
		{
			var em = new EntityManager();
			var entity = em.CreateEntity();
			var producer =  CreateWidgetFactory();
			entity.AddComponent( producer);

			var inventory = new Inventory();
			entity.AddComponent( inventory);

			var productionSystem = new ProductionSystem(em, new MockLogger(), new MockMap());
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

			var inventory = new Inventory();
			inventory.Size = 10;
			entity.AddComponent( inventory);

			var productionSystem = new ProductionSystem(em, new MockLogger(), new MockMap());
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
			var inventory = new Inventory();
			inventory.Size = 10;
			entity.AddComponent( inventory);
			var productionSystem = new ProductionSystem(em, new MockLogger(), new MockMap());
			productionSystem.Run();

			var item = inventory.Items[0];
			var name = item.GetComponent<Item>().Type;
			Assert.AreEqual(name, producer.ProducedItems[0].product);
		}

		[Test]
		public void DemandTest()
		{
			var em = new EntityManager();
			var entity = em.CreateEntity();
			var producer =  CreateWidgetFactory();
			entity.AddComponent(producer);
			var inventory = new Inventory();
			inventory.Size = 10;
			entity.AddComponent( inventory);
			var productionSystem = new ProductionSystem(em, new MockLogger(), new MockMap());
			productionSystem.Run();

			var demand = new Demand();
			demand.Demands.Add("widget", 1);
			entity.AddComponent( demand);
			var demandSystem = new DemandSystem(em, new MockLogger(), new MockMap());
			demandSystem.Run();

			Assert.Less(inventory.Items.Count, 10);
		}

		[Test]
		public void StackSystemTest()
		{
			var em = new EntityManager();
			var entity = em.CreateEntity();
			var inventory = new Inventory();
			entity.AddComponent(inventory);
			var itemEntity = em.CreateEntity();
			var itemComponent = new Item(){Type = "widget"};
			itemEntity.AddComponent<Stackable>();
			itemEntity.AddComponent(itemComponent);
			inventory.Items.Add(itemEntity);

			itemEntity = em.CreateEntity();
			itemEntity.AddComponent(itemComponent);
			itemEntity.AddComponent<Stackable>();
			inventory.Items.Add(itemEntity);

			var stackSystem = new StackSystem(em, new MockLogger(), new MockMap());
			var stacks = stackSystem.GetStacks(entity);

			Assert.AreEqual(2, stacks["widget"]);
		}

		// * //////////////// Helpers /////////////////
		Producer CreateWidgetFactory()
		{
			var producer = new Producer();
			var item = new Producer.ProductionItem(){product = "widget", rate = 11};
			producer.ProducedItems.Add(item);
			return producer;
		}

	}
}
