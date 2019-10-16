using System;
using System.Collections.Generic;
using static EntityComponentSystemCSharp.EntityManager;

namespace EntityComponentSystemCSharp.Components
{
	public interface IComponent
	{

	}

	public class ActorComponent : IComponent
	{

	}

	public class LocationComponent : IComponent
	{
		public int X = -1;
		public int Y = -1;
	}

	public class DemandComponent : IComponent
	{
		public Dictionary<string,int> Demands = new Dictionary<string, int>();
	}
	public class ItemComponent : IComponent
	{
		public string Type = "";
	}
	public class TagComponent : IComponent
	{
		public object Tag = null;
	}
	public class ProducerComponent: IComponent
	{
		public List<ProductionItem> ProducedItems = new List<ProductionItem>();

		public class ProductionItem
			{
			public string product = null;
			public float rate = 0.0f;
			public float inprogress = 0;
		}
	}

	public class InventoryComponent: IComponent
	{
		public List<Entity> Items = new List<Entity>();
		public int Size = int.MaxValue;
	}

	public class StackableComponent : IComponent
	{
	}
}