using System;
using System.Collections.Generic;
using RogueSharp;
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

	public class WanderingMonster : IComponent
	{
	}

	public class Attacked : IComponent
	{
		public Entity attacker;
	}

	public class Faction : IComponent
	{
		public MegaDungeon.Contracts.Factions Type;
	}

	public class AttackStat : IComponent
	{
		public int Power;
		public int Accuracy;
	}

	public class DefenseStat : IComponent
	{
		public int Chance;
	}

	public class SightStat : IComponent
	{
		public int Range;
	}

	public class Dead : IComponent
	{

	}
	
	public class Life : IComponent
	{
		public int Health;
		public int MaxHealth;
	}

	public class IsDoor : IComponent
	{
		public Orientation Orientation;
		public bool IsOpen;
	}

	public class Destination : PointWrapper, IComponent
	{

	}

	public class Player : IComponent
	{

	}

	public class Actor : IComponent
	{
		public float Speed;
		public int Gold;
		public float Energy;
	}

	public class Glyph : IComponent
	{
		public int glyph = -1;
	}

	public class Location : PointWrapper, IComponent
	{
	}

	public class Demand : IComponent
	{
		public Dictionary<string, int> Demands = new Dictionary<string, int>();
	}
	public class Item : IComponent
	{
		public string Type = "";
	}

	public class Name : IComponent
	{
		public string NameString = null;
	}

	public class Producer : IComponent
	{
		public List<ProductionItem> ProducedItems = new List<ProductionItem>();

		public class ProductionItem
		{
			public string product = null;
			public float rate = 0.0f;
			public float inprogress = 0;
		}
	}

	public class Inventory : IComponent
	{
		public List<Entity> Items = new List<Entity>();
		public int Size = int.MaxValue;
	}

	public class Stackable : IComponent
	{
	}
}