using System.Collections.Generic;
using EntityComponentSystemCSharp.Components;
using static EntityComponentSystemCSharp.EntityManager;

namespace EntityComponentSystemCSharp
{
	/// <summary>
	/// Convienence methods for dealing with EntityManager entities.
	/// </summary>
	public static class EntityExtensions
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>List of components attached to entity</returns>
		public static IEnumerable<IComponent> GetComponents(this Entity entity)
		{
			return entity.Manager.GetAllComponentsOnEntity(entity);
		}

		/// <summary>
		/// Return the component of type T
		/// </summary>
		/// <typeparam name="T">null if empty</typeparam>
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

		/// <summary>
		/// Permanently remove this enity and all it's components from the EntityManager
		/// </summary>
		/// <param name="entity"></param>
		public static void Destroy(this Entity entity)
		{
			entity.Manager.DestroyEntity(entity);
		}

		public static void AddComponent(this Entity entity, IComponent component)
		{
			entity.Manager.AddComponent(entity, component);
		}

		/// <summary>
		/// Adds and returns a new type T component to the entity.
		/// </summary>
		/// <typeparam name="T">New component where T: IComponent</typeparam>
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

		/// <summary>
		/// Type check components to prevent random objects from being added.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		static void CheckComponentAndThrow<T>()
		{
			if(!typeof(IComponent).IsAssignableFrom(typeof(T)))
			{
				throw new NotComponentException("You must specify a type that implement IComponent");
			}
		}

	}

}