using System;
using System.Collections.Generic;
using CommerceSim;
using CommerceSim.Components;
using static CommerceSim.EntityManager;

namespace CommerceSim.Systems
{
	public interface ISystem
	{
		void Run();
	}

	public abstract class SystemBase: ISystem
	{
		protected readonly EntityManager _em;
		public SystemBase(EntityManager em)
		{
			_em = em;
		}
		public abstract void Run();

	}

	public class ProductionSystem : SystemBase, ISystem
	{
		public ProductionSystem(EntityManager em) : base(em)
		{
		}

		public override void Run()
		{
			var producers = _em.GetAllEntitiesWithComponent<ProducerComponent>();
			foreach (var producer in producers)
			{
				var production = producer.GetComponent<ProducerComponent>();

				foreach(var product in production.ProducedItems)
				{
					product.inprogress += product.rate;
					var wholeItems = (int)(product.inprogress);
					product.inprogress =- wholeItems;
					var inventory = producer.GetComponent<InventoryComponent>();
					var numToAdd = Math.Min(inventory.Size, wholeItems);
					for(int i = 0; i < numToAdd; i++)
					{
						var item = _em.CreateEntity();
						item.AddComponent(new ItemComponent(){Type = product.product});
						inventory.Items.Add(item);
					}
				}
			}
		}
	}

	public class DemandSystem : SystemBase, ISystem
	{
		public DemandSystem(EntityManager em) : base(em)
		{
		}

		public override void Run()
		{
			var entities = _em.GetAllEntitiesWithComponent<DemandComponent>();
			foreach(var e in entities)
			{
				var demand = e.GetComponent<DemandComponent>();
				var inventory = e.GetComponent<InventoryComponent>();
				if(demand != null)
				{
					foreach(var d in demand.Demands)
					{
						var items = GetItemsFromInventory(e, d.Key);
						var numToRemove = Math.Min(items.Length, d.Value);
						for(int i = 0; i < numToRemove; i++)
						{
							inventory.Items.Remove(items[i]);
						}
					}
				}
			}
		}

		Entity[] GetItemsFromInventory(Entity entity, string type)
		{
			var inventory = entity.GetComponent<InventoryComponent>();
			if(inventory == null)
			{
				throw new MissingComponentException("Production requires an inventory for storage.");
			}
			var returnList = new List<Entity>();
			foreach(var item in inventory.Items)
			{
				var itemComponent = item.GetComponent<ItemComponent>();
				if(itemComponent != null && itemComponent.Type == type)
				{
					returnList.Add(item);
				}
			}
			return returnList.ToArray();
		}
	}

	public class StackSystem : SystemBase, ISystem
	{
		public StackSystem(EntityManager em) : base(em)
		{
			// TODO maybe compact stacks? InventoryItem would need a count.
		}

		public override void Run()
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, int> GetStacks(Entity entity)
		{
			var results = new Dictionary<string, int>();
			var inventory = entity.GetComponent<InventoryComponent>();
			if(inventory != null)
			{
				foreach(var item in inventory.Items)
				{
					var itemComponent = item.GetComponent<ItemComponent>();

					if(itemComponent != null && item.HasComponent<StackableComponent>())
					{
						if(!results.ContainsKey(itemComponent.Type))
						{
							results.Add(itemComponent.Type, 0);
						}
						results[itemComponent.Type] += 1;
					}
				}
			}
			return results;
		}
	}
}