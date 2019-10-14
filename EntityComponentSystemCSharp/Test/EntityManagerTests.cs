using System.Linq;
using NUnit.Framework;
using CommerceSim.Components;

namespace CommerceSim
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
			var tag = new TagComponent();
			entity.AddComponent(tag);
			entity.Destroy();
			var entities = _em.GetAllEntitiesWithComponent<TagComponent>();
			Assert.AreEqual(0, entities.Count);
		}

		[Test]
		public void AddAndGetComponentTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new TagComponent(){Tag = "test"};
			entity.AddComponent(tag);
			var actaul = entity.GetComponent<TagComponent>();
			Assert.AreEqual(tag, actaul);
		}

		[Test]
		public void RemoveComponentTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new TagComponent(){Tag = "test"};
			entity.AddComponent(tag);
			entity.RemoveComponent(tag);
			var actual = entity.GetComponent<TagComponent>();
			Assert.Null(actual);

			entity.AddComponent(tag);
			entity.RemoveComponent<TagComponent>();
			actual = entity.GetComponent<TagComponent>();
			Assert.Null(actual);
		}

		[Test]
		public void RemoveComponentGenericTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new TagComponent(){Tag = "test"};
			entity.AddComponent(tag);
			entity.RemoveComponent<TagComponent>();
			var actual = entity.GetComponent<TagComponent>();
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
			var tag = new TagComponent(){Tag = "test"};
			var producer = new ProducerComponent();
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
			var tag = new TagComponent(){Tag = "test"};
			var producer = new ProducerComponent();
			entity.AddComponent(tag);
			entity.AddComponent(producer);
			var actual = entity.GetComponent<TagComponent>();
			Assert.AreEqual(tag, actual);
		}

		[Test]
		public void GetAllComponentsOfTypeTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new TagComponent(){Tag = "test"};
			var producer = new ProducerComponent();
			entity.AddComponent(tag);
			entity.AddComponent(producer);

			var entity2 = _em.CreateEntity();
			var tag2 = new TagComponent(){Tag = "test2"};
			var producer2 = new ProducerComponent();
			entity2.AddComponent(tag2);
			entity2.AddComponent(producer2);

			var tagComponents = _em.GetAllComponentsOfType<TagComponent>();
			Assert.AreEqual(2, tagComponents.Count());
			Assert.AreEqual(tagComponents.First().GetType(), typeof(TagComponent));
		}

		[Test]
		public void GetAllEntitiesWithComponentTest()
		{
			var _em = new EntityManager();
			_em.CreateEntity();
			var entity = _em.CreateEntity();
			var tag = new TagComponent(){Tag = "test"};
			var producer = new ProducerComponent();
			entity.AddComponent(tag);
			entity.AddComponent(producer);

			var entity2 = _em.CreateEntity();
			var producer2 = new ProducerComponent();
			entity2.AddComponent(producer2);

			var entities = _em.GetAllEntitiesWithComponent<ProducerComponent>();
			Assert.AreEqual(2, entities.Count);
			Assert.IsTrue(entities.Contains(entity));
			Assert.IsTrue(entities.Contains(entity2));
		}

		[Test]
		public void HasComponentTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag = new TagComponent(){Tag = "test"};
			entity.AddComponent(tag);
			var hasTag = _em.HasComponent<TagComponent>(entity);
			Assert.IsTrue(hasTag);

			var hasProducer = _em.HasComponent<ProducerComponent>(entity);
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
			var tag = new TagComponent();
			entity.AddComponent(tag);
			var entities = _em.GetAllEntitiesWithComponent<TagComponent>();
			Assert.AreEqual(0, entities.Count);
		}

		[Test]
		public void UnfreezeTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			_em.Freeze();
			_em.UnFreeze();
			var tag = new TagComponent();
			entity.AddComponent(tag);
			var entities = _em.GetAllEntitiesWithComponent<TagComponent>();
			Assert.AreEqual(1, entities.Count);
		}

		[Test]
		public void DuplicateComponentsTest()
		{
			var _em = new EntityManager();
			var entity = _em.CreateEntity();
			var tag1 = new TagComponent();
			var tag2 = new TagComponent();
			entity.AddComponent(tag1);
			Assert.That(( ) => entity.AddComponent(tag2), Throws.TypeOf<DuplicateComponentException>());
		}
	}
}