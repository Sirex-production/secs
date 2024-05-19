using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		private Dictionary<Type, object> _singletonPools = new();
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object GetSingletonPoolAsObject(Type componentType)
		{
			if(!componentType.IsValueType)
				throw new EcsException(this, "Pool can not be created with non struct component");

			if (_singletonPools.ContainsKey(componentType))
				return _singletonPools[componentType];

			var poolType = typeof(EcsSingletonPool<>);
			var poolTypeWithGenericParameter = poolType.MakeGenericType(componentType);
			var poolInstance = Activator.CreateInstance(poolTypeWithGenericParameter);
			
			_singletonPools.Add(componentType, poolInstance);

			return poolInstance;
		}
		
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EcsSingletonPool<T> GetSingletonPool<T>() where T : struct, IEcsSingletonComponent
		{
			Type type = typeof(T);
		
			if (!_singletonPools.TryGetValue(type, out object pool))
			{
				pool = new EcsSingletonPool<T>();
				_singletonPools.Add(type, pool);
			}

			return (EcsSingletonPool<T>) pool;
		}
	}
}