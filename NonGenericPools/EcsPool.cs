using System.Runtime.CompilerServices;

namespace Secs
{
	public partial class EcsPool<T> : IEcsPoolGeneric<T>, IEcsPoolNonGeneric where T : struct, IEcsComponent
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEcsComponent GetCopyVirtual(in int entityId)
		{
			return GetCopy(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetVirtual(in int entityId, IEcsComponent cmp)
		{
			Set(entityId, cmp);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddVirtual(in int entityId, IEcsComponent cmp)
		{
			Add(entityId, cmp);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DelVirtual(in int entityId)
		{
			Del(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasVirtual(in int entityId)
		{
			return Has(entityId);
		}
	}
}