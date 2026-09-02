using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace Secs.Tests
{
	public sealed class EcsFilterTest
	{
		private struct CmpA : IEcsComponent { }
		private struct CmpB : IEcsComponent { }
		private struct CmpC : IEcsComponent { }

		[Test]
		public void WhenFilterIsCreatedWithIncludeTypes_AndEcsWorldHasEntitiesWithSuchTypes_ThenFilterShouldContainThem()
		{
			//Arrange
			var matcher = Create.EcsMatcherWithIncludeTypes(typeof(CmpA), typeof(CmpB), typeof(CmpC));
			var (world, filter) = Create.EcsFilterWithItsWorld(matcher);
			
			var cmpAPool = world.GetPool<CmpA>();
			var cmpBPool = world.GetPool<CmpB>();
			var cmpCPool = world.GetPool<CmpC>();
			int newEntity = world.NewEntity();
			
			//Act
			cmpAPool.Add(newEntity);
			cmpBPool.Add(newEntity);
			cmpCPool.Add(newEntity);
			
			//Assert
			foreach(int filterEntity in filter)
			{
				AreEqual(newEntity, filterEntity);
				IsTrue(cmpAPool.Has(filterEntity));
				IsTrue(cmpBPool.Has(filterEntity));
				IsTrue(cmpCPool.Has(filterEntity));
			}
		}

		[Test]
		public void WhenFilterIsCreatedWithIncludeAndExcludeTypes_AndEcsWorldHasEntitiesWithOnlyIncludeTypes_AndEcsWorldHasEntitiesWithExcludeTypes_ThenFilterShouldHaveOnlyEntitiesWithIncludeType()
		{
			//Arrange
			var includedTypes = new[] { typeof(CmpA), typeof(CmpB) };
			var excludedTypes = new[] { typeof(CmpC) };
			var matcher = Create.EcsMatcherWithIncludedAndExcludedTypes(includedTypes, excludedTypes);
			var (world, filter) = Create.EcsFilterWithItsWorld(matcher);
			
			var cmpAPool = world.GetPool<CmpA>();
			var cmpBPool = world.GetPool<CmpB>();
			var cmpCPool = world.GetPool<CmpC>();
			int includedEntity = world.NewEntity();
			int excludedEntity = world.NewEntity();
			
			//Act
			cmpAPool.Add(includedEntity);
			cmpBPool.Add(includedEntity);
			
			cmpAPool.Add(excludedEntity);
			cmpBPool.Add(excludedEntity);
			cmpCPool.Add(excludedEntity);
			
			//Assert
			IsTrue(filter.EntitiesCount == 1);
			
			foreach(int filterEntity in filter)
			{
				AreEqual(includedEntity, filterEntity);
				
				IsTrue(cmpAPool.Has(filterEntity));
				IsTrue(cmpBPool.Has(filterEntity));
				IsFalse(cmpCPool.Has(filterEntity));
			}
		}

		[Test]
		public void WhenEcsWorldWasCreated_AndEntitiesWithDifferentComponentsWereCreated_AndThenFilterWasCreated_ThenFilterShouldContainOnlyMatchingEntities()
		{
			//Arrange
			var includedTypes = new[] { typeof(CmpA), typeof(CmpB) };
			var excludedTypes = new[] { typeof(CmpC) };
			var matcher = Create.EcsMatcherWithIncludedAndExcludedTypes(includedTypes, excludedTypes);
			
			var world = Create.EcsWorld();
			var cmpAPool = world.GetPool<CmpA>();
			var cmpBPool = world.GetPool<CmpB>();
			var cmpCPool = world.GetPool<CmpC>();
			
			int includedCmp = world.NewEntity();
			int excludedCmp = world.NewEntity();
			
			//Act
			cmpAPool.Add(includedCmp);
			cmpBPool.Add(includedCmp);
			
			cmpAPool.Add(excludedCmp);
			cmpBPool.Add(excludedCmp);
			cmpCPool.Add(excludedCmp);
			
			var filter = world.GetFilter(matcher);
			
			//Assert
			AreEqual(1, filter.EntitiesCount);
			
			foreach(int filterEntity in filter)
			{
				AreEqual(includedCmp, filterEntity);
				
				IsTrue(cmpAPool.Has(filterEntity));
				IsTrue(cmpBPool.Has(filterEntity));
				IsFalse(cmpCPool.Has(filterEntity));
			}
		}
		
		[Test]
		public void WhenFilterIsCreated_AndEcsWorldHasSomeEntitiesThatShouldBeIncluded_AndThenComponentsWereDeletedFromEntity_ThenFilterShouldNotContainSuchEntities()
		{
			//Arrange
			var matcher = Create.EcsMatcherWithIncludeTypes(typeof(CmpA), typeof(CmpB));

			var (world, filter) = Create.EcsFilterWithItsWorld(matcher);
			var cmpAPool = world.GetPool<CmpA>();
			var cmpBPool = world.GetPool<CmpB>();

			int includedCmp = world.NewEntity();
			int excludedCmp = world.NewEntity();
			
			//Act
			cmpAPool.Add(includedCmp);
			cmpBPool.Add(includedCmp);
			cmpAPool.Add(excludedCmp);
			cmpBPool.Add(excludedCmp);
			
			cmpBPool.Del(excludedCmp);
			
			//Assert
			IsTrue(filter.EntitiesCount == 1);
			
			foreach(int filterEntity in filter)
			{
				AreEqual(includedCmp, filterEntity);
				
				IsTrue(cmpAPool.Has(filterEntity));
				IsTrue(cmpBPool.Has(filterEntity));
			}
		}

		[Test]
		public void WhenFilterIsCreated_AndEcsWorldHasSomeEntitiesThatShouldBeIncluded_AndThenEntityWasDeleted_ThenFilterShouldNotHaveDeletedEntities()
		{
			//Arrange
			var matcher = Create.EcsMatcherWithIncludeTypes(typeof(CmpA), typeof(CmpB));
			var (world, filter) = Create.EcsFilterWithItsWorld(matcher);
			var cmpAPool = world.GetPool<CmpA>();
			var cmpBPool = world.GetPool<CmpB>();
			
			int aliveEntity = world.NewEntity();
			int deadEntity = world.NewEntity();
			
			cmpAPool.Add(aliveEntity);
			cmpBPool.Add(aliveEntity);
			
			cmpAPool.Add(deadEntity);
			cmpBPool.Add(deadEntity);
			
			//Act
			world.DelEntity(deadEntity);
			
			//Assert
			IsTrue(filter.EntitiesCount == 1);
			
			foreach(int filterEntity in filter)
			{
				AreEqual(aliveEntity, filterEntity);
				
				IsTrue(cmpAPool.Has(filterEntity));
				IsTrue(cmpBPool.Has(filterEntity));
			}
		}
	}
}