using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T AddCmp<T>(in int entityId) where T : struct, IEcsComponent
		{
			return ref GetPool<T>().AddComponent(entityId);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetCmp<T>(in int entityId) where T : struct, IEcsComponent
		{
			return ref GetPool<T>().GetComponent(entityId);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DelCmp<T>(in int entityId) where T : struct, IEcsComponent
		{
			GetPool<T>().DelComponent(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasCmp<T>(in int entityId) where T : struct, IEcsComponent
		{
			return GetPool<T>().HasComponent(entityId);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T NewEntityWithCmp<T>(out int entityId) where T : struct, IEcsComponent
		{
			entityId = NewEntity();
			return ref GetPool<T>().AddComponent(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetFirstEntityWithCmp<T>() where T : struct, IEcsComponent
		{
			var matcher = EcsMatcher.Include(typeof(T)).End();
			return GetFilter(matcher).GetFirstEntity();
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetFirstCmp<T>() where T : struct, IEcsComponent
		{
			var matcher = EcsMatcher.Include(typeof(T)).End();
			var entity = GetFilter(matcher).GetFirstEntity();
			
			return ref GetPool<T>().GetComponentWithNonRefArguments(entity);
		}
	}
}