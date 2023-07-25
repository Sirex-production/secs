using System;

namespace Secs
{
	public static class Create
	{
		public static EcsTypeMask EcsTypeMaskWithTypes(params Type[] componentTypes)
		{
			return new EcsTypeMask(componentTypes);
		}
		
		public static EcsTypeMask EcsTypeMaskWithEmptyTypes()
		{
			return new EcsTypeMask(Type.EmptyTypes);
		}
		
		public static EcsTypeMask EcsTypeMaskEmpty()
		{
			return new EcsTypeMask();
		}

		public static EcsTypeMask EcsTypeMaskNull()
		{
			return null;
		}

		public static EcsMatcher EcsMatcherWithIncludeTypes(params Type[] types)
		{
			return EcsMatcher
				.Include(types)
				.End();
		}

		public static EcsMatcher EcsMatcherWithNoTypes()
		{
			return EcsMatcher
				.Include(null)
				.End();
		}
		
		public static EcsMatcher EcsMatcherWithIncludedAndExcludedTypes(Type[] includedTypes, Type[] excludedTypes)
		{
			return EcsMatcher
				.Include(includedTypes)
				.Exclude(excludedTypes)
				.End();
		}

		public static EcsMatcher EcsMatcherNull()
		{
			return null;
		}
		
		public static (EcsWorld world, EcsFilter filter) EcsFilterWithItsWorld(EcsMatcher matcher)
		{
			var world = new EcsWorld();
			var filter = world.GetFilter(matcher);
			return (world, filter);
		}

		public static EcsWorld EcsWorld()
		{
			return new EcsWorld();
		}
	}
}