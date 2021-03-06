using System.Linq;
using NUnit.Framework;
using EntityComponentSystemCSharp.Components;

namespace EntityComponentSystemCSharp
{
	[TestFixture]
	public class EntityManagerTests
	{

		[Test]
		public void CreateEntityTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			Assert.IsNotNull(entity);
		}

		[Test]
		public void KillEntityTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new Name();
			entity.AddComponent(tag);
			entity.Destroy();
			var entities = _em.GetAllEntitiesWithComponent<Name>();
			Assert.AreEqual(0, entities.Count);
		}

		[Test]
		public void AddAndGetComponentTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new Name(){NameString = "test"};
			entity.AddComponent(tag);
			var actaul = entity.GetComponent<Name>();
			Assert.AreEqual(tag, actaul);
		}

		[Test]
		public void RemoveComponentTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new Name(){NameString = "test"};
			entity.AddComponent(tag);
			entity.RemoveComponent(tag);
			var actual = entity.GetComponent<Name>();
			Assert.Null(actual);

			entity.AddComponent(tag);
			entity.RemoveComponent<Name>();
			actual = entity.GetComponent<Name>();
			Assert.Null(actual);
		}

		[Test]
		public void RemoveComponentGenericTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new Name(){NameString = "test"};
			entity.AddComponent(tag);
			entity.RemoveComponent<Name>();
			var actual = entity.GetComponent<Name>();
			Assert.Null(actual);
		}

		[Test]
		public void NotComponentException()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			Assert.That(( ) => entity.GetComponent<System.Object>(), Throws.TypeOf<NotComponentException>());
		}

		[Test]
		public void GetAllComponentsOnEntityTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new Name(){NameString = "test"};
			var producer = new Producer();
			entity.AddComponent(tag);
			entity.AddComponent(producer);
			var allComponents = entity.GetComponents();
			Assert.AreEqual(2, allComponents.Count());
		}

		[Test]
		public void GetComponentOfTypeTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new Name(){NameString = "test"};
			var producer = new Producer();
			entity.AddComponent(tag);
			entity.AddComponent(producer);
			var actual = entity.GetComponent<Name>();
			Assert.AreEqual(tag, actual);
		}

		[Test]
		public void GetAllComponentsOfTypeTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new Name(){NameString = "test"};
			var producer = new Producer();
			entity.AddComponent(tag);
			entity.AddComponent(producer);

			var entity2 = _em.CreateEntity();
			var tag2 = new Name(){NameString = "test2"};
			var producer2 = new Producer();
			entity2.AddComponent(tag2);
			entity2.AddComponent(producer2);

			var tagComponents = _em.GetAllComponentsOfType<Name>();
			Assert.AreEqual(2, tagComponents.Count());
			Assert.AreEqual(tagComponents.First().GetType(), typeof(Name));
		}

		[Test]
		public void GetAllEntitiesWithComponentTest()
		{
			var _em = new EntityManager();
			_em.CreateEntity();
			var entity = _em.CreateEntity();
			var tag = new Name(){NameString = "test"};
			var producer = new Producer();
			entity.AddComponent(tag);
			entity.AddComponent(producer);

			var entity2 = _em.CreateEntity();
			var producer2 = new Producer();
			entity2.AddComponent(producer2);

			var entities = _em.GetAllEntitiesWithComponent<Producer>();
			Assert.AreEqual(2, entities.Count);
			Assert.IsTrue(entities.Contains(entity));
			Assert.IsTrue(entities.Contains(entity2));
		}

		[Test]
		public void HasComponentTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new Name(){NameString = "test"};
			entity.AddComponent(tag);
			var hasTag = _em.HasComponent<Name>(entity);
			Assert.IsTrue(hasTag);

			var hasProducer = _em.HasComponent<Producer>(entity);
			Assert.IsFalse(hasProducer);
		}

		[Test]
		public void FreezeTest()
		{
			var _em = new EntityManager();
			_em.Freeze();
			var negaEntity = _em.CreateEntity();
			var count = _em.Entities.Count();
			Assert.AreEqual(0, count);
			_em.UnFreeze();

			var entity = _em.CreateEntity();
			_em.Freeze();
			var tag = new Name();
			entity.AddComponent(tag);
			var entities = _em.GetAllEntitiesWithComponent<Name>();
			Assert.AreEqual(0, entities.Count);
		}

		[Test]
		public void UnfreezeTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			_em.Freeze();
			_em.UnFreeze();
			var tag = new Name();
			entity.AddComponent(tag);
			var entities = _em.GetAllEntitiesWithComponent<Name>();
			Assert.AreEqual(1, entities.Count);
		}

		[Test]
		public void DuplicateComponentsTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag1 = new Name();
			var tag2 = new Name();
			entity.AddComponent(tag1);
			Assert.That(( ) => entity.AddComponent(tag2), Throws.TypeOf<DuplicateComponentException>());
		}
	}
}