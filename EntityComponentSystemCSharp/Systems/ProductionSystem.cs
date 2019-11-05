using System;
using EntityComponentSystemCSharp.Components;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
	public class ProductionSystem : SystemBase, ISystem
	{
		public ProductionSystem(IEngine engine) : base(engine)
		{
		}

		public override void Run(EntityManager.Entity entity)
		{
			if(!entity.HasComponent<Producer>()){return;}

			var production = entity.GetComponent<Producer>();

			foreach(var product in production.ProducedItems)
			{
				product.inprogress += product.rate;
				var wholeItems = (int)(product.inprogress);
				product.inprogress =- wholeItems;
				var inventory = entity.GetComponent<Inventory>();
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