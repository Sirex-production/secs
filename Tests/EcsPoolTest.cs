using System;
using NUnit.Framework;

namespace Secs.Tests
{
	public class EcsPoolTest
	{
		private struct CmpA : IEcsComponent
		{
			public int value;
		}

#region GenericApi
		[Test]
		public void WhenComponentIsNotAddedToTheEntity_ThenItShouldNotBePresent()
		{
			//Arrange
			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();

			//Act && Assert
			Assert.IsFalse(pool.HasComponent(entity));
		}

		[Test]
		public void WhenComponentIsAddedToTheEntity_WithGenericApi_ThenItShouldBePresentThere()
		{
			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();

			//Act
			pool.AddComponent(entity);

			//Assert
			Assert.IsTrue(pool.HasComponent(entity));
		}

		[Test]
		public void WhenComponentIsAddedToTheEntity_WithGenericApi_ThenWeShouldBeAbleToGetItWithTheSameData()
		{
			//Arrange
			const int constantCmpValue = 33;

			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();

			//Act
			pool.AddComponent(entity) = new CmpA
			{
				value = constantCmpValue
			};

			//Assert
			Assert.AreEqual(pool.GetComponent(entity).value, constantCmpValue);
		}

		[Test]
		public void WhenComponentValueIsModified_WithGenericApi_ThenWeShouldBeAbleToGetItWithModifiedData()
		{
			//Arrange
			const int constantInitialCmpValue = 5;
			const int constantNewCmpValue = 33;

			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();
			var initialComponent = new CmpA
			{
				value = constantInitialCmpValue
			};

			//Act
			pool.AddComponent(entity) = initialComponent;
			ref var cmpA = ref pool.GetComponent(entity);
			cmpA.value = constantNewCmpValue;

			//Assert
			Assert.AreEqual(pool.GetComponent(entity).value, constantNewCmpValue);
		}

		[Test]
		public void WhenComponentIsDeletedFromTheEntity_ThenItShouldNotBePresent()
		{
			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();

			//Act
			pool.AddComponent(entity);
			pool.DelComponent(entity);

			//Assert
			Assert.IsFalse(pool.HasComponent(entity));
		}
#endregion

#region NonGenericApi
		[Test]
		public void WhenComponentIsAddedToTheEntity_WithNonGenericApi_ThenWeShouldBeAbleToGetItWithTheSameData()
		{
			//Arrange
			const int constantCmpValue = 33;

			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();
			var component = new CmpA
			{
				value = constantCmpValue
			};

			//Act
			pool.AddComponent(entity, component);
			var cmpACopy = (CmpA)pool.GetComponentCopy(entity);

			//Assert
			Assert.AreEqual(cmpACopy.value, constantCmpValue);
		}

		[Test]
		public void WhenComponentIsSetToTheEntity_WithNonGenericApi_ThenWeShouldBeAbleToGetItWithModifiedData()
		{
			//Arrange
			const int constantInitialCmpValue = 5;
			const int constantNewCmpValue = 33;

			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();
			var initialComponent = new CmpA
			{
				value = constantInitialCmpValue
			};
			var newComponent = new CmpA
			{
				value = constantNewCmpValue
			};

			//Act
			pool.AddComponent(entity, initialComponent);
			pool.SetComponent(entity, newComponent);

			//Assert
			var cmpACopy = (CmpA)pool.GetComponentCopy(entity);
			Assert.AreEqual(cmpACopy.value, constantNewCmpValue);
		}
#endregion

#region SparseSetStorage
		[Test]
		public void WhenComponentIsDeleted_WithOtherEntitiesHavingComponents_ThenTheirValuesShouldStayIntact()
		{
			//Arrange
			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entityA = world.NewEntity();
			int entityB = world.NewEntity();
			int entityC = world.NewEntity();

			pool.AddComponent(entityA) = new CmpA { value = 1 };
			pool.AddComponent(entityB) = new CmpA { value = 2 };
			pool.AddComponent(entityC) = new CmpA { value = 3 };

			//Act
			pool.DelComponent(entityB);

			//Assert
			Assert.AreEqual(pool.GetComponent(entityA).value, 1);
			Assert.AreEqual(pool.GetComponent(entityC).value, 3);
		}

		[Test]
		public void WhenComponentIsReadded_WithPreviousValueDeleted_ThenItShouldBeDefault()
		{
			//Arrange
			const int constantCmpValue = 33;

			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();
			int otherEntity = world.NewEntity();

			pool.AddComponent(otherEntity) = new CmpA { value = constantCmpValue };
			pool.AddComponent(entity) = new CmpA { value = constantCmpValue };

			//Act
			pool.DelComponent(entity);
			int reusedEntity = world.NewEntity();
			pool.AddComponent(reusedEntity);

			//Assert
			Assert.AreEqual(pool.GetComponent(reusedEntity).value, 0);
		}

		[Test]
		public void WhenEntityIdIsReused_WithComponentOnDeletedEntity_ThenComponentShouldNotBePresent()
		{
			//Arrange
			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();

			pool.AddComponent(entity) = new CmpA { value = 33 };

			//Act
			world.DelEntity(entity);
			int reusedEntity = world.NewEntity();

			//Assert
			Assert.AreEqual(entity, reusedEntity);
			Assert.IsFalse(pool.HasComponent(reusedEntity));
		}

		[Test]
		public void WhenComponentsAreAddedAndDeleted_WithCountExceedingInitialCapacity_ThenValuesShouldStayIntact()
		{
			//Arrange
			const int constantEntitiesAmount = 40;

			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			var entities = new int[constantEntitiesAmount];

			//Act
			for (int i = 0; i < constantEntitiesAmount; i++)
			{
				entities[i] = world.NewEntity();
				pool.AddComponent(entities[i]) = new CmpA { value = i };
			}

			for (int i = 0; i < constantEntitiesAmount; i += 2)
				pool.DelComponent(entities[i]);

			//Assert
			for (int i = 1; i < constantEntitiesAmount; i += 2)
				Assert.AreEqual(pool.GetComponent(entities[i]).value, i);
		}

		[Test]
		public void WhenLastComponentIsDeleted_WithEntityHavingNoOtherComponents_ThenEntityShouldBeAutoDeletedWithoutThrowing()
		{
			//Arrange
			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();

			pool.AddComponent(entity);

			//Act && Assert
			Assert.DoesNotThrow(() => pool.DelComponent(entity));
			Assert.IsTrue(world.IsEntityDead(entity));
		}

		[Test]
		public void WhenComponentIsAdded_WithListenerAddingComponentToTheSamePool_ThenReturnedReferenceShouldPointToTheRightComponent()
		{
			//Arrange
			const int constantOuterCmpValue = 111;
			const int constantInnerCmpValue = 222;

			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int entity = world.NewEntity();
			int otherEntity = world.NewEntity();
			bool hasListenerFired = false;

			void OnComponentAdded(int addedToEntity, Type componentType)
			{
				if(hasListenerFired)
					return;

				hasListenerFired = true;
				pool.AddComponent(otherEntity) = new CmpA { value = constantInnerCmpValue };
			}

			world.OnComponentAddedToEntity += OnComponentAdded;

			//Act
			pool.AddComponent(entity) = new CmpA { value = constantOuterCmpValue };
			world.OnComponentAddedToEntity -= OnComponentAdded;

			//Assert
			Assert.AreEqual(pool.GetComponent(entity).value, constantOuterCmpValue);
			Assert.AreEqual(pool.GetComponent(otherEntity).value, constantInnerCmpValue);
		}

		[Test]
		public void WhenComponentIsAddedToHighEntityId_WithManyEntitiesWithoutComponent_ThenDenseStorageShouldHoldSingleComponent()
		{
			//Arrange
			const int constantEntitiesAmount = 101;

			var world = Create.EcsWorld();
			var pool = world.GetPool<CmpA>();
			int lastEntity = -1;

			for (int i = 0; i < constantEntitiesAmount; i++)
				lastEntity = world.NewEntity();

			//Act
			pool.AddComponent(lastEntity);

			//Assert
			Assert.AreEqual(lastEntity, constantEntitiesAmount - 1);
			Assert.AreEqual(pool.Count, 1);
		}
#endregion
	}
}