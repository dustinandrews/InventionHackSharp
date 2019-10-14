using System.Collections.Generic;
using CommerceSim.Components;
using static CommerceSim.EntityManager;

namespace CommerceSim
{
	public static class EntityExtensions
	{
		public static IEnumerable<IComponent> GetComponents(this Entity entity)
		{
			return entity.Manager.GetAllComponentsOnEntity(entity);
		}

		public static T GetComponent<T>(this Entity entity) where T: class
		{
			return entity.Manager.GetComponent<T>(entity);
		}

		public static void RemoveComponent<T>(this Entity entity) where T: class
		{
			entity.Manager.RemoveComponent<T>(entity);
		}

		public static void RemoveComponent(this Entity entity, IComponent component)
		{
			entity.Manager.RemoveComponent(entity.Id, component);
		}

		public static void Destroy(this Entity entity)
		{
			entity.Manager.DestroyEntity(entity);
		}

		public static void AddComponent(this Entity entity, IComponent component)
		{
			entity.Manager.AddComponent(entity, component);
		}

		/// <summary>
		/// Adds and returns a new T() component to the entity.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static T AddComponent<T>(this Entity entity) where T: class, new()
		{
			CheckComponentAndThrow<T>();
			var component = (IComponent) new T();
			entity.AddComponent(component);
			return (T) component;
		}

		public static bool HasComponent<T>(this Entity entity) where T: class
		{
			CheckComponentAndThrow<T>();
			var component = entity.GetComponent<T>();
			return component != null;
		}

		static void CheckComponentAndThrow<T>()
		{
			if(!typeof(IComponent).IsAssignableFrom(typeof(T)))
			{
				throw new NotComponentException("You must specify a type that implement IComponent");
			}
		}

	}

}