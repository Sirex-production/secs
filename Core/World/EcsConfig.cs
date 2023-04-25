namespace Secs
{
	public struct EcsConfig
	{
		public static EcsConfig Default => new EcsConfig
		{
			world = new WorldConfig
			{
				initialAllocatedEntities = 64,
				initialAllocatedEntityUpdateOperations = 32,
				initialAllocatedPools = 16,
				initialAllocatedFilters = 16
			},
			pool = new PoolConfig
			{
				initialAllocatedComponents = 16
			},
			filter = new FilterConfig
			{
				initialAllocatedEntities = 16
			}
		};

		public WorldConfig world;
		public PoolConfig pool;
		public FilterConfig filter;
		
		public struct WorldConfig
		{
			public int initialAllocatedEntities;
			public int initialAllocatedEntityUpdateOperations;
			public int initialAllocatedPools;
			public int initialAllocatedFilters;
		}
		
		public struct PoolConfig
		{
			public int initialAllocatedComponents;
		}
		
		public struct FilterConfig
		{
			public int initialAllocatedEntities;
		}
	}
}