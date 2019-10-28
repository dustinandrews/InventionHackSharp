using System;
using System.Collections.Generic;
using EntityComponentSystemCSharp;
using EntityComponentSystemCSharp.Components;
using static EntityComponentSystemCSharp.EntityManager;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
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

	public class MovementSystem : SystemBase, ISystem
	{
		IMap _map;
		RogueSharp.PathFinder _pathfinder;
		public MovementSystem(EntityManager em, IMap map) : base(em)
		{
			_map = map;
			_pathfinder = new RogueSharp.PathFinder(_map, 1);
		}

		public override void Run()
		{
			var placedEntities = _em.GetAllEntitiesWithComponent<Location>();
			foreach(var entity in _em.GetAllEntitiesWithComponent<Destination>())
			{
				var current = entity.GetComponent<Location>();
				var desired = entity.GetComponent<Destination>();
				if(_map != null && desired != null)
				{
					var start = _map.GetCell(current.X, current.Y);
					var dest = _map.GetCell(desired.X, desired.Y);
					var path = _pathfinder.TryFindShortestPath(start, dest);
					if(path != null)
					{
						var next = path.StepForward();
						foreach(var e in placedEntities)
						{
							var l = e.GetComponent<Location>();
							// TODO: Do something other than swapping.
							if( l.X == next.X && l.Y == next.Y)
							{
								l.X = current.X;
								l.Y = current.Y;
							}
						}
						current.X = next.X;
						current.Y = next.Y;
					}
				}
			}
		}
	}

	public class ProductionSystem : SystemBase, ISystem
	{
		public ProductionSystem(EntityManager em) : base(em)
		{
		}

		public override void Run()
		{
			var producers = _em.GetAllEntitiesWithComponent<Producer>();
			foreach (var producer in producers)
			{
				var production = producer.GetComponent<Producer>();

				foreach(var product in production.ProducedItems)
				{
					product.inprogress += product.rate;
					var wholeItems = (int)(product.inprogress);
					product.inprogress =- wholeItems;
					var inventory = producer.GetComponent<Inventory>();
					var numToAdd = Math.Min(inventory.Size, wholeItems);
					for(int i = 0; i < numToAdd; i++)
					{
						var item = _em.CreateEntity();
						item.AddComponent(new Item(){Type = product.product});
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
			var entities = _em.GetAllEntitiesWithComponent<Demand>();
			foreach(var e in entities)
			{
				var demand = e.GetComponent<Demand>();
				var inventory = e.GetComponent<Inventory>();
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
			var inventory = entity.GetComponent<Inventory>();
			if(inventory == null)
			{
				throw new MissingComponentException("Production requires an inventory for storage.");
			}
			var returnList = new List<Entity>();
			foreach(var item in inventory.Items)
			{
				var itemComponent = item.GetComponent<Item>();
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
			var inventory = entity.GetComponent<Inventory>();
			if(inventory != null)
			{
				foreach(var item in inventory.Items)
				{
					var itemComponent = item.GetComponent<Item>();

					if(itemComponent != null && item.HasComponent<Stackable>())
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