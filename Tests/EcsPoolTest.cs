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
	}
}