using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		/// <summary>
		/// Finds pool and uses it to add component to the entity.
		/// </summary>
		/// <param name="entityId">Entity to which component will be added</param>
		/// <typeparam name="T">Type of component</typeparam>
		/// <returns>Added component</returns>
		/// <remarks>Try to use cached pool instead in very frequent operations since it is faster</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Add<T>(in int entityId) where T : struct, IEcsComponent
		{
			return ref GetPool<T>().Add(entityId);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddVoid<T>(in int entityId) where T : struct, IEcsComponent
		{
			GetPool<T>().Add(entityId);
		}
		/// <summary>
		/// Finds pool and uses it to get component from the entity.
		/// </summary>
		/// <param name="entityId">Entity from which component will be taken</param>
		/// <typeparam name="T">Type of component</typeparam>
		/// <returns>Found attached component</returns>
		/// <remarks>Try to use cached pool instead in very frequent operations since it is faster</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>(in int entityId) where T : struct, IEcsComponent
		{
			return ref GetPool<T>().Get(entityId);
		}
		
		/// <summary>
		/// Finds pool and uses it to delete component from the entity.
		/// </summary>
		/// <param name="entityId">Entity from which component will be deleted</param>
		/// <typeparam name="T">Type of component</typeparam>
		/// <remarks>Try to use cached pool instead in very frequent operations since it is faster</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Del<T>(in int entityId) where T : struct, IEcsComponent
		{
			GetPool<T>().Del(entityId);
		}
		
		public void TryDel<T>(in int entityId) where T : struct, IEcsComponent
		{
			GetPool<T>().TryDel(entityId);
		}

		public ref T GetOrAdd<T>(in int entityId) where T : struct, IEcsComponent
		{
			return ref GetPool<T>().GetOrAdd(entityId);
		}

		/// <summary>
		/// Finds pool and uses it to check if entity has component of given type.
		/// </summary>
		/// <param name="entityId">Entity that will be checked</param>
		/// <typeparam name="T">Type of component</typeparam>
		/// <returns>TRUE if component is present. FALSE otherwise</returns>
		/// <remarks>Try to use cached pool instead in very frequent operations since it is faster</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>(in int entityId) where T : struct, IEcsComponent
		{
			return GetPool<T>().Has(entityId);
		}
		
		/// <summary>
		/// Creates new entity and attaches component to it
		/// </summary>
		/// <param name="entityId">Created entity</param>
		/// <typeparam name="T">Type of component</typeparam>
		/// <returns>Added component</returns>
		/// <remarks>Try to use cached pool instead in very frequent operations since it is faster</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T NewEntityWithCmp<T>(out int entityId) where T : struct, IEcsComponent
		{
			entityId = NewEntity();
			return ref GetPool<T>().Add(entityId);
		}

		/// <summary>
		/// Finds filter with given component and gets first entity from it 
		/// </summary>
		/// <param name="entityId">Retrieved entity</param>
		/// <typeparam name="T">Type of component that must be present on entity</typeparam>
		/// <returns>TRUE if entity was found, FALSE otherwise</returns>
		/// <remarks>Try to use cached filter instead in very frequent operations since it is faster</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetFirstEntityWithCmp<T>(out int entityId) where T : struct, IEcsComponent
		{
			var matcher = EcsMatcher.Include(typeof(T)).End();
			var filter = GetFilter(matcher);
			
			if(filter.IsEmpty)
			{
				entityId = -1;
				return false;
			}
			
			entityId = filter.GetFirstEntity();
			return true;
		}
		
		/// <summary>
		/// Finds filter and pool with specified component and returns component of first entity from filter
		/// </summary>
		/// <param name="component">Component that will be found</param>
		/// <typeparam name="T">Type of component</typeparam>
		/// <returns>TRUE if such component was found. FALSE otherwise</returns>
		/// <remarks>Try to use cached filter and pool instead in very frequent operations since it is faster</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetFirstCmp<T>(out T component) where T : struct, IEcsComponent
		{
			if(TryGetFirstEntityWithCmp<T>(out int entityId))
			{
				component = GetPool<T>().Get(entityId);
				return true;
			}

			component = default;
			return false;
		}
		
		/// <summary>
		/// Finds filter and pool with specified component and returns component of first entity from filter
		/// </summary>
		/// <param name="component">Component that will be found</param>
		/// <param name="entityId">Entity from which component was taken</param>
		/// <typeparam name="T">Type of component</typeparam>
		/// <returns>TRUE if such component was found. FALSE otherwise</returns>
		/// <remarks>Try to use cached filter and pool instead in very frequent operations since it is faster</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetFirstCmp<T>(out T component, out int entityId) where T : struct, IEcsComponent
		{
			if(TryGetFirstEntityWithCmp<T>(out int firstEntityId))
			{
				component = GetPool<T>().Get(firstEntityId);
				entityId = firstEntityId;
				return true;
			}

			component = default;
			entityId = -1;
			return false;
		}

		/// <summary>
		/// Gets 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetSingletonCmp<T>() where T : struct, IEcsSingletonComponent
		{
			return ref GetSingletonPool<T>().Component;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetSingletonEntity<T>() where T : struct, IEcsSingletonComponent
		{
			return GetSingletonPool<T>().OwnerEntity;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T AddSingletonCmp<T>(int entityId) where T : struct, IEcsSingletonComponent
		{
			return ref GetSingletonPool<T>().Add(entityId);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsSingletonPresent<T>() where T : struct, IEcsSingletonComponent
		{
			if(!_singletonPools.ContainsKey(typeof(T)))
				return false;

			return GetSingletonPool<T>().IsPresent;
		}
	}
}