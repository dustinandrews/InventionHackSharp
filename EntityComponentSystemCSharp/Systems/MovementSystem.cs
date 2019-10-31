using System.Linq;
using EntityComponentSystemCSharp.Components;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
	public class MovementSystem : SystemBase, ISystem
	{
		RogueSharp.PathFinder _pathfinder;
		public MovementSystem(EntityManager em, ISystemLogger logger, RogueSharp.IMap map) : base(em, logger, map)
		{
			_pathfinder = new RogueSharp.PathFinder(_map, 1);
		}

		public override void Run(EntityManager.Entity entity)
		{
			if(!(entity.HasComponent<Destination>() &&
				entity.HasComponent<Actor>() &&
				entity.GetComponent<Actor>().Energy >= 10))
			{return;}
			
			var locationEntities = _em.GetAllEntitiesWithComponent<Location>()
				.Where(e => e.HasComponent<Actor>());

			
			entity.GetComponent<Actor>().Energy -= 10;
			var allComponents = entity.GetComponents();
			var current = entity.GetComponent<Location>();
			var desired = entity.GetComponent<Destination>();
			if(_map != null && desired != null)
			{
				var start = _map.GetCell(current.X, current.Y);
				var dest = _map.GetCell(desired.X, desired.Y);
				var path = _pathfinder.TryFindShortestPath(start, dest);
				var isClear = true;
				if(path != null && path.Length > 1)
				{
					var next = path.StepForward();
					foreach(var e in locationEntities)
					{
						var othersLocation = e.GetComponent<Location>();

						if( othersLocation.X == next.X && othersLocation.Y == next.Y)
						{
							var myFaction = entity.GetComponent<Faction>();
							var theirFaction = e.GetComponent<Faction>();
							if(myFaction != null &&
							theirFaction!= null &&
							myFaction.Type == theirFaction.Type)
							{
								othersLocation.X = current.X;
								othersLocation.Y = current.Y;
							}
							else
							{
									e.AddOrUpdateComponent(new Attacked(){attacker = entity});
									isClear = false; //Attacking cancels move
							}
						}
					}

					if (isClear)
					{
						current.X = next.X;
						current.Y = next.Y;
					}
				}
			}
		}
	}
}