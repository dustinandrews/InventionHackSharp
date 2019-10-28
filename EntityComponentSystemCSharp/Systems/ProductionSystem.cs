using System;
using EntityComponentSystemCSharp.Components;

namespace EntityComponentSystemCSharp.Systems
{
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
}