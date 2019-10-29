using System;
using System.Collections.Generic;
using EntityComponentSystemCSharp.Components;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
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