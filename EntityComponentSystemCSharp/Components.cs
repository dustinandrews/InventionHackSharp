using System;
using System.Collections.Generic;
using static EntityComponentSystemCSharp.EntityManager;

namespace EntityComponentSystemCSharp.Components
{
	/// <summary>
	/// Defines allowable components. Components SHOULD only have data members.
	/// Components SHOULD be simple to serialize.
	/// </summary>
	/// <remarks>
	/// An entity may one have ONE of a a give component type.
	/// Use collections of entities for aggregates.
	/// </remarks>
	public interface IComponent
	{

	}

	public class DestinationComponent : PointWrapper, IComponent
	{

	}

	public class PlayerComponent : IComponent
	{

	}

	public class ActorComponent : IComponent
	{

	}

	public class GlyphComponent : IComponent
	{
		public int glyph = -1;
	}

	public class LocationComponent : PointWrapper, IComponent
	{
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