using System;
using System.Linq;
using System.Reflection;
using Mono.CompilerServices.SymbolWriter;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		/// <summary>
		/// Gets pool of specific type 
		/// </summary>
		/// <param name="componentType">Type of component that pool holds</param>
		/// <returns>EcsPool of given type as object</returns>
		/// <exception cref="ArgumentException">If componentType is non struct</exception>
		private object GetPoolAsObject(Type componentType)
		{
			if(!componentType.IsValueType)
				throw new ArgumentException("Pool can not be created with non struct component");

			var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			int typeIndex = EcsTypeIndexUtility.GetIndexOfType(componentType);

			if (_pools.ContainsKey(typeIndex))
				return _pools[typeIndex];

			var poolType = typeof(EcsPool<>);
			var poolTypeWithGenericParameter = poolType.MakeGenericType(componentType);
			var poolInstance = Activator.CreateInstance(poolTypeWithGenericParameter, bindingFlags, null, new object[] {config.pool.initialAllocatedComponents, this}, null);
			
			_pools.Add(typeIndex, poolInstance);

			return poolInstance;
		}
		
		/// <summary>
		/// Injects EcsWorld into given object
		/// </summary>
		/// <param name="injectionTarget">Object to which fields will be injected</param>
		private void InjectWorld(object injectionTarget)
		{
			var fieldBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var injectFilterFields = injectionTarget.GetType()
				.GetFields(fieldBindings)
				.Where
				(
					fieldInfo => fieldInfo.IsDefined(typeof(EcsInjectAttribute)) && 
								fieldInfo.FieldType == typeof(EcsWorld)
				);

			foreach(var worldFiledInfo in injectFilterFields)
			{
				worldFiledInfo.SetValue(injectionTarget, this);
			}
		}
		
		/// <summary>
		/// Injects EcsFilters into given object
		/// </summary>
		/// <param name="injectionTarget">Object to which fields will be injected</param>
		private void InjectFilters(object injectionTarget)
		{
			var fieldBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var injectFilterFields = injectionTarget.GetType()
				.GetFields(fieldBindings)
				.Where
				(
					fieldInfo => fieldInfo.FieldType == typeof(EcsFilter) && 
								fieldInfo.IsDefined(typeof(EcsInjectAttribute))
				);

			foreach(var filterFieldInfo in injectFilterFields)
			{
				var includeTypes = filterFieldInfo.GetCustomAttribute<EcsInjectAttribute>().includeTypes;
				
				if(includeTypes == null || includeTypes.Length < 1)
					continue;
				
				EcsFilter ecsFilter;
				
				if(filterFieldInfo.IsDefined(typeof(AndExclude)))
				{
					var excludeTypes = filterFieldInfo.GetCustomAttribute<AndExclude>().excludeTypes;
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

		/// <summary>
		/// Injects EcsPools into given object
		/// </summary>
		/// <param name="injectionTarget">Object to which fields will be injected</param>
		private void InjectPools(object injectionTarget)
		{
			var fieldBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var injectPoolFields = injectionTarget.GetType()
				.GetFields(fieldBindings)
				.Where
				(
					fieldInfo => fieldInfo.IsDefined(typeof(EcsInjectAttribute)) &&
								fieldInfo.FieldType.IsGenericType 
				)
				.Where(fieldInfo => fieldInfo.FieldType.GetGenericTypeDefinition()  == typeof(EcsPool<>));

			foreach(var poolFieldInfo in injectPoolFields)
			{
				var componentType = poolFieldInfo.FieldType.GetGenericArguments()[0];
				var ecsPool = GetPoolAsObject(componentType);
				
				
				poolFieldInfo.SetValue(injectionTarget, ecsPool);
			}
		}

		/// <summary>
		/// Injects ECS types like pools, filters and world into fields of a given object 
		/// </summary>
		/// <param name="injectionTarget">Object to which fields will be injected</param>
		internal void Inject(object injectionTarget)
		{
			InjectWorld(injectionTarget);
			InjectFilters(injectionTarget);
			InjectPools(injectionTarget);
		}
	}
}