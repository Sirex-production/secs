using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace Secs
{
	public sealed class EcsMatcherTest
	{
		private struct CmpA : IEcsComponent { }
		private struct CmpB : IEcsComponent { }
		private struct CmpC : IEcsComponent { }
		
		[Test]
		public void WhenMatcherIsCreatedWithIncludeTypes_ThatTypesShouldBeIncluded()
		{
			//Arrange
			var matcher = Create.EcsMatcherWithIncludeTypes(typeof(CmpA), typeof(CmpB), typeof(CmpC));
			
			//Act
			
			//Assert
			IsTrue(matcher.IsIncluded(typeof(CmpA)));
			IsTrue(matcher.IsIncluded(typeof(CmpB)));
			IsTrue(matcher.IsIncluded(typeof(CmpC)));
		}
		
		[Test]
		public void WhenEmptyMatcherIsCreated_ThenNonOfTypesShouldBeIncluded()
		{
			//Arrange
			var matcher = Create.EcsMatcherWithNoTypes();

			//Act
			
			//Assert
			IsFalse(matcher.IsIncluded(typeof(CmpA)));
			IsFalse(matcher.IsIncluded(typeof(CmpB)));
			IsFalse(matcher.IsIncluded(typeof(CmpC)));
		}
		
		[Test]
		public void WhenMatcherIsCreatedWithIncludeTypes_AndWithExcludedTypes_ThenIncludedTypesShouldBeOnlyIncluded()
		{
			//Arrange
			var includeTypes = new []{typeof(CmpA), typeof(CmpB)};
			var excludeTypes = new []{typeof(CmpC)};
			var matcher = Create.EcsMatcherWithIncludedAndExcludedTypes(includeTypes, excludeTypes);
			
			//Act
			
			//Assert
			IsTrue(matcher.IsIncluded(typeof(CmpA)));
			IsTrue(matcher.IsIncluded(typeof(CmpB)));
			IsFalse(matcher.IsExcluded(typeof(CmpA)));
			IsFalse(matcher.IsExcluded(typeof(CmpB)));
		}
		
		[Test]
		public void WhenMatcherIsCreatedWithIncludeTypes_AndWithExcludedTypes_ThenExcludedTypesShouldBeOnlyExcluded()
		{
			//Arrange
			var includeTypes = new []{typeof(CmpA), typeof(CmpB)};
			var excludeTypes = new []{typeof(CmpC)};
			var matcher = Create.EcsMatcherWithIncludedAndExcludedTypes(includeTypes, excludeTypes);
			
			//Act

			//Assert
			IsTrue(matcher.IsExcluded(typeof(CmpC)));
			IsFalse(matcher.IsIncluded(typeof(CmpC)));
		}
		
		[Test]
		public void WhenTwoMatchersAreCreatedWithSameIncludeAndExcludeMasks_ThenTheseMatchersShouldBeEqual()
		{
			//Arrange
			var firstMatcher = Create.EcsMatcherWithIncludedAndExcludedTypes(new []{typeof(CmpA), typeof(CmpB)}, new []{typeof(CmpC)});
			var secondMatcher = Create.EcsMatcherWithIncludedAndExcludedTypes(new []{typeof(CmpA), typeof(CmpB)}, new []{typeof(CmpC)});
			
			//Act
			
			//Assert
			AreEqual(firstMatcher, secondMatcher);
		}

		[Test]
		public void WhenOneMatcherIsCreated_AndSecondMatcherIsNull_TheyShouldNotBeEqual()
		{
			//Arrange
			var firstMatcher = Create.EcsTypeMaskWithTypes(typeof(CmpA));
			var secondMatcher = Create.EcsMatcherNull();

			//Act
			
			//Assert
			AreNotEqual(firstMatcher, secondMatcher);
		}
	}
}