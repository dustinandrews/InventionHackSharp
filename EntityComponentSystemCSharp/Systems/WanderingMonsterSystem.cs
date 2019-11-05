using System;
using System.Collections.Generic;
using EntityComponentSystemCSharp.Components;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
	/// <summary>
	/// Entities with the wandering monster component will wander around at random
	/// and attack the player if they see them.
	/// </summary>
	public class WanderingMonsterSystem : SystemBase
	{
		Random rand = new Random();
		List<ICell> walkable = new List<ICell>();
		public WanderingMonsterSystem(IEngine engine): base(engine)
		{
			foreach(var cell in _map.GetAllCells())
			{
				if(cell.IsWalkable)
				{
					walkable.Add(cell);
				}
			}
		}

		public override void Run(EntityManager.Entity entity)
		{
			if(!entity.HasComponent<WanderingMonster>()){return;}
			if(!entity.HasComponent<Location>()){return;}

			var actual = entity.GetComponent<Location>();
			var desired = entity.GetComponent<Destination>();

			var point = new Point(actual.X, actual.Y);
			if (_engine.GetPlayerViewable().Contains(point))
			{
				desired = new Destination() {X =_engine.GetPlayerLocation().X, Y = _engine.GetPlayerLocation().Y};
				entity.AddOrUpdateComponent((IComponent)desired);
			}
			else if(desired == null || desired == actual)
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