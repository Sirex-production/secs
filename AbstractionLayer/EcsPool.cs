using System.Runtime.CompilerServices;

namespace Secs
{
	public partial class EcsPool<T> : IEcsPoolGeneric<T>, IEcsPoolNonGeneric where T : struct, IEcsComponent
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEcsComponent GetComponentCopyVirtual(in int entityId)
		{
			return GetComponentCopy(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetComponentVirtual(in int entityId, IEcsComponent cmp)
		{
			SetComponent(entityId, cmp);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddComponentVirtual(in int entityId, IEcsComponent cmp)
		{
			AddComponent(entityId, cmp);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DelComponentVirtual(in int entityId)
		{
			DelComponent(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasComponentVirtual(in int entityId)
		{
			return HasComponent(entityId);
		}
	}
}