using System;
using System.Linq;
using System.Collections.Generic;
using CommerceSim.Components;
using System.Text;

namespace CommerceSim
{
	public partial class EntityManager
	{
		// Entities are just and only integers. Entity is a wrapper with a reference back to the entity
		// manager to allow extension methods to work more fluently.
		public class Entity
		{
			private int _id;
			private EntityManager _manager;

			public int Id { get{ return _id; } }
			public EntityManager Manager { get { return _manager; } }

			public Entity(int id, EntityManager manager)
			{
				_id = id;
				_manager = manager;
			}

			public override string ToString()
			{
				return $"Id = {_id}";
			}
		}

		public IEnumerable<Entity> Entities => _entities.ToArray(); // RO version of current entities
		HashSet<Entity> _entities = new HashSet<Entity>();
		// key is "{entityInt}+{typeof(component).ToString()}"
		Dictionary<String, IComponent> _map = new Dictionary<String, IComponent>();
		bool isFrozen = false;

		public Entity CreateEntity()
		{
			if(isFrozen)
			{
			   return new Entity(id: -1, manager: this);
			}

			var id = 0;
			while(_entities.Any(e => e.Id == id))
			{
				id++;
			}

			var entity = new Entity(id, this);
			_entities.Add(entity);
			return entity;
		}

		public void DestroyEntity(Entity entity)
		{
			if(isFrozen)
			{
				return;
			}

			_entities.Remove(entity);
			List<string> removeKeys = new List<string>();
			foreach (var item in _map)
			{
				var pattern = $"{entity.Id}+";
				if(item.Key.StartsWith(pattern))
				{
					removeKeys.Add(item.Key);
				}
			}
			foreach(var key in removeKeys)
			{
				_map.Remove(key);
			}
		}

		public IEnumerable<T> GetAllComponentsOfType<T>() where T : class
		{
			var returnList = new List<T>();
			foreach(var entity in _entities)
			{
				var component = GetComponent<T>(entity);
				if(component != null)
				{
					returnList.Add((T)component);
				}
			}
			return returnList;
		}


		public T GetComponent<T>(Entity entity) where T: class
		{
			CheckComponentAndThrow<T>();
			return GetComponent<T>(entity.Id);
		}

		public T GetComponent<T>(int id) where T: class
		{
			var componentType = typeof(T);

			T returnVal = null;
			var key = GetMapKey(id, componentType);

			if(_map.ContainsKey(key))
			{
				returnVal = (T)_map[key];
			}
			return returnVal;
		}

		public List<IComponent> GetAllComponentsOnEntity(Entity entity)
		{
			return GetAllComponentsOnEntity(entity.Id);
		}
		private List<IComponent> GetAllComponentsOnEntity(int id)
		{
			var returnList = new List<IComponent>();
			foreach(var entry in _map)
			{
				if(entry.Key.StartsWith($"{id}+"))
				{
					returnList.Add(entry.Value);
				}
			}
			return returnList;
		}

		public List<Entity> GetAllEntitiesWithComponent<T>() where T : class
		{
			Type componentType = typeof(T);
			var returnList = new List<Entity>();
			foreach(var entity in _entities)
			{
				var component = GetComponent<T>(entity);
				if(component != null)
				{
					returnList.Add(entity);
				}
			}
			return returnList;
		}

		public void RemoveComponent<T>(Entity entity) where T: class
		{
			var component = (IComponent) GetComponent<T>(entity);
			RemoveComponent(entity.Id, component);
		}

		public void RemoveComponent(int id, IComponent component)
		{
			if(isFrozen)
			{
				return;
			}

			var key = GetMapKey(id, component.GetType());
			if(_map.ContainsKey(key))
			{
				_map.Remove(key);
			}
		}

		public void AddComponent(Entity entity, IComponent component)
		{
			AddComponent(entity.Id, component);
		}
		public void AddComponent(int entityId, IComponent component)
		{
			if(isFrozen)
			{
				return;
			}

			var key = GetMapKey(entityId, component.GetType());
			if(!_map.ContainsKey(key))
			{
				_map.Add(key, component);
			}
			else
			{
				throw new DuplicateComponentException($"Entity already has a component of type {component.GetType()}. Modify existing or remove first.");
			}
		}

		internal bool HasComponent<T>(Entity entity) where T : class
		{
			var component = GetComponent<T>(entity);
			return component != null;
		}

		public void Freeze()
		{
			isFrozen = true;
		}

		public void UnFreeze()
		{
			isFrozen = false;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach(var item in _map)
			{
				sb.AppendLine(item.Key);
			}
			return sb.ToString();
		}

		string GetMapKey(int entity, Type componentType)
		{
			return $"{entity}+{componentType.ToString()}";
		}

		void CheckComponentAndThrow<T>()
		{
			if(!typeof(IComponent).IsAssignableFrom(typeof(T)))
			{
				throw new NotComponentException("You must specify a type that implement IComponent");
			}
		}
	}
}