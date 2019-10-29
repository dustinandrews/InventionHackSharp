using System;
using System.Collections.Generic;
using EntityComponentSystemCSharp;
using EntityComponentSystemCSharp.Components;
using static EntityComponentSystemCSharp.EntityManager;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
	public abstract class SystemBase: ISystem
	{
		protected readonly EntityManager _em;
		protected readonly ISystemLogger _logger;
		protected readonly IMap _map;
		public SystemBase(EntityManager em, ISystemLogger logger, IMap map)
		{
			_em = em;
			_logger = logger;
			_map = map;
		}
		public abstract void Run();
	}

	public class RandomMovementSystem : SystemBase
	{
		Random rand = new Random();
		List<ICell> walkable = new List<ICell>();
		public RandomMovementSystem(EntityManager em, ISystemLogger logger, IMap map): base(em, logger, map)
		{
			foreach(var cell in map.GetAllCells())
			{
				if(cell.IsWalkable)
				{
					walkable.Add(cell);
				}
			}
		}

		public override void Run()
		{
			foreach(var entity in _em.GetAllEntitiesWithComponent<RandomMovement>())
			{
				var actual = entity.GetComponent<Location>();
				var desired = entity.GetComponent<Destination>();
				if(desired == null || desired == actual)
				{
					entity.RemoveComponent<Destination>();
					var randCell = walkable[rand.Next(0, walkable.Count)];
					desired = entity.AddComponent<Destination>();
					desired.X = randCell.X;
					desired.Y = randCell.Y;
				}
			}
		}
	}
}