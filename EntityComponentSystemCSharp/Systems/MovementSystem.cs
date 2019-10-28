using EntityComponentSystemCSharp.Components;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
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
}