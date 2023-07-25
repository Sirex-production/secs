using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace Secs
{
	public sealed class EcsTypeMaskTest
	{
		private struct CmpA : IEcsComponent { }
		private struct CmpB : IEcsComponent { }
		private struct CmpC : IEcsComponent { }

		[Test]
		public void WhenMaskWasCreatedWithOneType_ThenMaskShouldContainThatType()
		{
			//Arrange
			var typeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA));
			
			//Act
			
			//Assert
			IsTrue(typeMask.ContainsType<CmpA>());
		}

		[Test]
		public void WhenMaskWasCreatedWithOneType_AndThenAnotherTypeWasAddedToThisMask_ThenMaskShouldContainTheseTypes()
		{
			//Arrange
			var typeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA));
			
			//Act
			typeMask.AddType<CmpB>();
			
			//Assert
			IsTrue(typeMask.ContainsType<CmpA>());
			IsTrue(typeMask.ContainsType<CmpB>());
		}
		
		[Test]
		public void WhenTwoMasksWereCreated_AndSecondMaskTypesIsASubsetOfFirstMaskTypes_ThenFirstMaskShouldIncludeSecondMask()
		{
			//Arrange
			var firstTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpB));
			var secondTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA));
			
			//Act
			
			//Assert
			IsTrue(firstTypeMask.Includes(secondTypeMask));
		}
		
		[Test]
		public void WhenTwoIdenticalMasksWereCreated_ThenTheyShouldIncludeEachOther()
		{
			//Arrange
			var firstTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpB));
			var secondTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpB));
			
			//Act
			
			//Assert
			IsTrue(firstTypeMask.Includes(secondTypeMask));
			IsTrue(secondTypeMask.Includes(firstTypeMask));
		}
		
		[Test]
		public void WhenTwoMasksWereCreated_AndSecondMaskTypesIsNotASubsetOfFirstMaskTypes_ThenFirstMaskShouldNotIncludeSecondMask()
		{
			//Arrange
			var firstTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpB));
			var secondTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpC));
			
			//Act
			
			//Assert
			IsFalse(firstTypeMask.Includes(secondTypeMask));
		}
		
		[Test]
		public void WhenTwoMasksWereCreated_AndTheyHaveAtLeastOneCommonType_ThenTheyShouldHaveCommonTypes()
		{
			//Arrange
			var firstTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpB));
			var secondTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpC), typeof(CmpB));
			
			//Act
			
			//Assert
			True(firstTypeMask.HasCommonTypesWith(secondTypeMask));
			True(secondTypeMask.HasCommonTypesWith(firstTypeMask));
		}

		[Test]
		public void WhenMaskIsCreatedWithOneType_AndThenAnotherTypeWasAdded_ThenMaskShouldContainBothTypes()
		{
			//Arrange
			var typeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA));
			
			//Act
			typeMask.AddType<CmpB>();
			
			//Assert
			IsTrue(typeMask.ContainsType<CmpA>());
			IsTrue(typeMask.ContainsType<CmpB>());
		}
		
		[Test]
		public void WhenMaskIsCreatedWithFirstType_AndThenSecondTypeWasRemoved_ThenMaskShouldContainOnlyFirstType()
		{
			//Arrange
			var typeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpB));
			
			//Act
			typeMask.RemoveType<CmpB>();
			
			//Assert
			IsTrue(typeMask.ContainsType<CmpA>());
			IsFalse(typeMask.ContainsType<CmpB>());
		}
		
		[Test]
		public void WhenMaskIsCreatedWithoutAnyTypes_ThenMaskShouldNotContainAnyTypes()
		{
			//Arrange
			var typeMask = Create.EcsTypeMaskEmpty();
			
			//Act
			
			//Assert
			IsFalse(typeMask.HasAnyTypes());
		}
		
		[Test]
		public void WhenMaskWithEmptyTypesWasCreated_ThenMaskShouldNotContainAnyTypes()
		{
			//Arrange
			var typeMask = Create.EcsTypeMaskWithEmptyTypes();
			
			//Act
			
			//Assert
			IsFalse(typeMask.HasAnyTypes());
		}
		
		[Test]
		public void WhenMaskIsCreatedWithOneType_ThenMaskShouldContainAnyTypes()
		{
			//Arrange
			var typeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA));
			
			//Act
			
			//Assert
			IsTrue(typeMask.HasAnyTypes());
		}
		
		[Test]
		public void WhenTwoMasksAreCreatedWithSameTypes_ThenTheyShouldBeEqual()
		{
			//Arrange
			var firstTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpB));
			var secondTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpB), typeof(CmpA));
			
			//Act
			
			//Assert
			AreEqual(firstTypeMask, secondTypeMask);
		}
		
		[Test]
		public void WhenTwoMasksAreCreatedWithDifferentTypes_ThenTheyShouldNotBeEqual()
		{
			//Arrange
			var firstTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpB));
			var secondTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpB), typeof(CmpC));
			
			//Act
			
			//Assert
			AreNotEqual(firstTypeMask, secondTypeMask);
		}
		
		[Test]
		public void WhenOneMaskIsCreated_AndSecondMaskIsNull_ThenTheyShouldNotBeEqual()
		{
			//Arrange
			var firstTypeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpB));
			var secondTypeMask = Create.EcsTypeMaskNull();
			
			//Act
			
			//Assert
			AreNotEqual(firstTypeMask, secondTypeMask);
			AreNotEqual(secondTypeMask, firstTypeMask);
		}
		
		[Test]
		public void WhenTypeMaskWasCreated_AndObjectOfAnotherTypeWasCreated_ThenTheyShouldNotBeEqual()
		{
			//Arrange
			var typeMask = Create.EcsTypeMaskWithTypes(typeof(CmpA), typeof(CmpB));
			var anotherObject = new object();
			
			//Act
			
			//Assert
			AreNotEqual(typeMask, anotherObject);
		}
	}
}