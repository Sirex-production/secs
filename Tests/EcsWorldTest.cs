using NUnit.Framework;

namespace Secs.Tests
{
	public class EcsWorldTest
	{
		private struct CmpA : IEcsComponent { }
		private class BadCmpType_Class : IEcsComponent { }
		private class BadCmpType_NonIEcsComponent : IEcsComponent { }

#region GenericApi
		[Test]
		public void WhenGettingPool_WithGenericApi_NonNullPoolShouldBeReturned()
		{
			//Arrange
			var world = Create.EcsWorld();

			//Act
			var pool = world.GetPool<CmpA>();

			//Assert
			Assert.IsNotNull(pool);
		}
#endregion

#region NonGenericApi
		[Test]
		public void WhenGettingPool_WithNonGenericApi_OfWrongCmpType_ThenEcsExceptionShouldBeThrown()
		{
			//Arrange
			var world = Create.EcsWorld();
			
			//Act && Assert
			Assert.Throws<EcsException>(() => world.GetPool(typeof(BadCmpType_Class)));
			Assert.Throws<EcsException>(() => world.GetPool(typeof(BadCmpType_NonIEcsComponent)));
		}

		[Test]
		public void WhenGettingPool_WithNonGenericApi_ThenNonNullPoolShouldBeReturned()
		{
			//Arrange
			var world = Create.EcsWorld();

			//Act
			var pool = world.GetPool(typeof(CmpA));

			//Assert
			Assert.IsNotNull(pool);
		}
		
		[Test]
		public void WhenGettingPool_WithNonGenericApi_ThenPoolOfCorrectTypeShouldBeReturned()
		{
			//Arrange
			var world = Create.EcsWorld();

			//Act
			var pool = world.GetPool(typeof(CmpA));

			//Assert
			Assert.IsTrue(pool is EcsPool<CmpA>);
		}
#endregion
	}
}