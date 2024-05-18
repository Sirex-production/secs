using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		private Dictionary<Type, object> _singletonPools = new();
		
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