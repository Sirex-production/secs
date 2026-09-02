#if TOOLS
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		private static readonly Dictionary<(Type, string), MethodInfo> SingletonMethods = new();

		internal IEcsComponent GetItem(Type cmpType, int entity)
		{
			return GetNonGenericPool(cmpType).GetCopyVirtual(entity);
		}

		internal void ReplaceItem(Type cmpType, int entity, IEcsComponent cmp)
		{
			GetNonGenericPool(cmpType).SetVirtual(entity, cmp);
		}

		internal void AddItem(Type cmpType, int entity, IEcsComponent cmp)
		{
			GetNonGenericPool(cmpType).AddVirtual(entity, cmp);
		}

		internal void DelItem(Type cmpType, int entity)
		{
			GetNonGenericPool(cmpType).DelVirtual(entity);
		}

		internal bool HasItem(Type cmpType, int entity)
		{
			return GetNonGenericPool(cmpType).HasVirtual(entity);
		}

		internal bool IsSingletonPresent(Type cmpType)
		{
			return (bool)GetSingletonMethod(cmpType, nameof(IsSingletonPresentInternal)).Invoke(this, null);
		}

		internal object GetSingletonItem(Type cmpType)
		{
			return GetSingletonMethod(cmpType, nameof(GetSingletonItemInternal)).Invoke(this, null);
		}

		internal void SetSingletonItem(Type cmpType, object cmp)
		{
			GetSingletonMethod(cmpType, nameof(SetSingletonItemInternal)).Invoke(this, new[] { cmp });
		}

		internal void DelSingletonItem(Type cmpType)
		{
			GetSingletonMethod(cmpType, nameof(DelSingletonItemInternal)).Invoke(this, null);
		}

		private bool IsSingletonPresentInternal<T>() where T : struct, IEcsSingletonComponent
		{
			return GetSingletonPool<T>().IsPresent;
		}

		private T GetSingletonItemInternal<T>() where T : struct, IEcsSingletonComponent
		{
			return GetSingletonPool<T>().Component;
		}

		private void SetSingletonItemInternal<T>(T cmp) where T : struct, IEcsSingletonComponent
		{
			GetSingletonPool<T>().Component = cmp;
		}

		private void DelSingletonItemInternal<T>() where T : struct, IEcsSingletonComponent
		{
			GetSingletonPool<T>().Del();
		}

		private static MethodInfo GetSingletonMethod(Type cmpType, string methodName)
		{
			var key = (cmpType, methodName);

			if(SingletonMethods.TryGetValue(key, out var method))
				return method;

			method = typeof(EcsWorld)
				.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)
				.MakeGenericMethod(cmpType);

			SingletonMethods[key] = method;

			return method;
		}
	}
}
#endif
