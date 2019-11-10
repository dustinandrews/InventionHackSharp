using System.Collections.Generic;
using System.Linq;
using EntityComponentSystemCSharp.Components;
using static EntityComponentSystemCSharp.EntityManager;
using RogueSharp;

namespace EntityComponentSystemCSharp.Systems
{
	public class MovementSystem : SystemBase, ISystem
	{
		bool _isClear = true;
		RogueSharp.PathFinder _pathfinder;
		int _nsOpenDoorGlyph, _ewOpenDoorGlyph;
		public MovementSystem(IEngine engine) : base(engine)
		{
			_pathfinder = new RogueSharp.PathFinder(_map, 1);
			var tm = _engine.GetTileManager();
			_nsOpenDoorGlyph = tm.GetGlyphNumByName("open door NS");
			_ewOpenDoorGlyph = tm.GetGlyphNumByName("open door EW");
		}

		public override void Run(Entity entity)
		{
			if(!(entity.HasComponent<Destination>() &&
				entity.HasComponent<Actor>() &&
				entity.GetComponent<Actor>().Energy >= 10))
			{return;}
			
			var locationEntities = _em.GetAllEntitiesWithComponent<Location>()
				.Where(e => e.HasComponent<Actor>() || e.HasComponent<IsDoor>());

			entity.GetComponent<Actor>().Energy -= 10;
			var allComponents = entity.GetComponents();
			var current = entity.GetComponent<Location>();
			var desired = entity.GetComponent<Destination>();
			if(_map != null && desired != null && desired.X >= 0 && desired.X < _map.Width && desired.Y >= 0 && desired.Y < _map.Height)
			{
				var start = _map.GetCell(current.X, current.Y);
				var dest = _map.GetCell(desired.X, desired.Y);
				var path = _pathfinder.TryFindShortestPath(start, dest);
				_isClear = true;
				if(path != null && path.Length > 1)
				{
					var next = path.StepForward();
					ResolveCollisions(entity, locationEntities, current, next);
					if (_isClear)
					{
						current.X = next.X;
						current.Y = next.Y;
					}
				}
			}
		}


		void ResolveCollisions(Entity entity, IEnumerable<Entity> locationEntities, Location current, ICell next)
		{
			_isClear = true;
			foreach(var other in locationEntities)
			{
				var othersLocation = other.GetComponent<Location>();
				if (othersLocation.X == next.X && othersLocation.Y == next.Y)
				{
					var handled = DoorCheck(other, othersLocation);
					if(!handled){ResolveAttack(entity, current, other, othersLocation);}
				}
			}
		}

		bool DoorCheck(Entity other, Location othersLocation)
		{
			var handled = false;
			if(other.HasComponent<IsDoor>())
			{
				var door = other.GetComponent<IsDoor>();
				var glyph = other.GetComponent<Glyph>();

				// TODO: Check for locked doors.

				door.IsOpen = true;
				if(door.Orientation == Orientation.N || door.Orientation == Orientation.S)
				{
					glyph.glyph = _nsOpenDoorGlyph;
				}
				else
				{
					glyph.glyph = _ewOpenDoorGlyph;
				}
				_map.SetCellProperties(othersLocation.X, othersLocation.Y, isTransparent: true, isWalkable: true);
				handled = true;
			}
			return handled;
		}

		bool ResolveAttack(Entity entity, Location current, Entity other, Location othersLocation)
		{
			var handled = false;
			var myFaction = entity.GetComponent<Faction>();
			var theirFaction = other.GetComponent<Faction>();
			var theirAttacked = other.GetComponent<Attacked>();

			// Check to see if the two are eligible to swap.
			// Must be same faction and target not under (unresolved) attack.
			if (myFaction != null &&
			theirFaction != null &&
			theirAttacked == null &&
			myFaction.Type == theirFaction.Type)
			{
				othersLocation.X = current.X;
				othersLocation.Y = current.Y;
				handled = true;
			}
			else
			{
				other.AddOrUpdateComponent(new Attacked() { attacker = entity });
				_isClear = false; //Attacking cancels move
				handled = true;
			}
			return handled;
		}
	}
}