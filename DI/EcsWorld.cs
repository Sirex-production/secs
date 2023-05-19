using System;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		private object GetPoolAsObject(Type componentType)
		{
			if(!componentType.IsValueType)
				throw new ArgumentException("Pool can not be created with non struct component");
			
			int typeIndex = EcsTypeIndexUtility.GetIndexOfType(componentType);

			if (_pools.ContainsKey(typeIndex))
				return _pools[typeIndex];

			var poolType = Type.GetType("Secs.EcsPool`1");
			var poolTypeWithGenericParameter = poolType.MakeGenericType(componentType);
			var poolInstance = Activator.CreateInstance(poolTypeWithGenericParameter, config.pool.initialAllocatedComponents, this);
			
			_pools.Add(typeIndex, poolInstance);

			return poolInstance;
		}
		
		private void InjectPools(object injectionTarget)
		{
			var fieldBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var injectPoolFields = injectionTarget.GetType()
				.GetFields(fieldBindings)
				.Where(fieldInfo => fieldInfo.IsDefined(typeof(InjectEcsPoolAttribute)) && fieldInfo.FieldType == typeof(EcsPool<>));

			foreach(var poolFieldInfo in injectPoolFields)
			{
				var componentType = poolFieldInfo.GetCustomAttribute<InjectEcsPoolAttribute>().componentType;
				var ecsPool = GetPoolAsObject(componentType);
				
				poolFieldInfo.SetValue(injectionTarget, ecsPool);
			}
		}

		private void InjectFilters(object injectionTarget)
		{
			var fieldBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var injectFilterFields = injectionTarget.GetType()
				.GetFields(fieldBindings)
				.Where
				(
					fieldInfo => fieldInfo.FieldType == typeof(EcsFilter) && 
								fieldInfo.IsDefined(typeof(InjectEcsFilterAttribute)) &&
								fieldInfo.IsDefined(typeof(Inc))
				);

			foreach(var filterFieldInfo in injectFilterFields)
			{
				var includeTypes = filterFieldInfo.GetCustomAttribute<Inc>().includeTypes;
				EcsFilter ecsFilter;
				
				if(filterFieldInfo.IsDefined(typeof(Exc)))
				{
					var excludeTypes = filterFieldInfo.GetCustomAttribute<Exc>().excludeTypes;
					var ecsMatcher = EcsMatcher
						.Include(includeTypes)
						.Exclude(excludeTypes)
						.End();
					
					ecsFilter = GetFilter(ecsMatcher);
				}
				else
				{
					var ecsMatcher = EcsMatcher
						.Include(includeTypes)
						.End();
					
					ecsFilter = GetFilter(ecsMatcher);
				}
				
				filterFieldInfo.SetValue(injectionTarget, ecsFilter);
			}
		}
		
		internal void Inject(object injectionTarget)
		{
			InjectPools(injectionTarget);
			InjectFilters(injectionTarget);
		}
	}
}