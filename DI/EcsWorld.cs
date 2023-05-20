using System;
using System.Linq;
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

			var poolType = typeof(EcsPool<>);
			var poolTypeWithGenericParameter = poolType.MakeGenericType(componentType);
			var poolInstance = Activator.CreateInstance(poolTypeWithGenericParameter, config.pool.initialAllocatedComponents, this);
			
			_pools.Add(typeIndex, poolInstance);

			return poolInstance;
		}
		
		private void InjectWorld(object injectionTarget)
		{
			var fieldBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var injectFilterFields = injectionTarget.GetType()
				.GetFields(fieldBindings)
				.Where
				(
					fieldInfo => fieldInfo.IsDefined(typeof(EcsWorldInject)) && 
								fieldInfo.FieldType == typeof(EcsWorld)
				);

			foreach(var worldFiledInfo in injectFilterFields)
			{
				worldFiledInfo.SetValue(injectionTarget, this);
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
								fieldInfo.IsDefined(typeof(EcsFilterInjectAttribute)) &&
								fieldInfo.IsDefined(typeof(EcsIncludeAttribute))
				);

			foreach(var filterFieldInfo in injectFilterFields)
			{
				var includeTypes = filterFieldInfo.GetCustomAttribute<EcsIncludeAttribute>().includeTypes;
				EcsFilter ecsFilter;
				
				if(filterFieldInfo.IsDefined(typeof(EcsExcludeAttribute)))
				{
					var excludeTypes = filterFieldInfo.GetCustomAttribute<EcsExcludeAttribute>().excludeTypes;
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

		private void InjectPools(object injectionTarget)
		{
			var fieldBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var injectPoolFields = injectionTarget.GetType()
				.GetFields(fieldBindings)
				.Where
				(
					fieldInfo => fieldInfo.IsDefined(typeof(EcsPoolInjectAttribute)) &&
								fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(EcsPool<>) 
				);

			foreach(var poolFieldInfo in injectPoolFields)
			{
				var componentType = poolFieldInfo.FieldType.GetGenericArguments()[0];
				var ecsPool = GetPoolAsObject(componentType);
				
				
				poolFieldInfo.SetValue(injectionTarget, ecsPool);
			}
		}

		internal void Inject(object injectionTarget)
		{
			InjectWorld(injectionTarget);
			InjectFilters(injectionTarget);
			InjectPools(injectionTarget);
		}
	}
}