using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed class EcsSingletonPool<T> where T : struct, IEcsSingletonComponent
	{
		private T _component;
		private int _ownerEntityId;

		public bool IsPresent
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _ownerEntityId > -1;
		}

		public int OwnerEntity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _ownerEntityId;
		}
		
		public ref T Component
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _component;
		}

		public ref T AddComponent(int entityId)
		{
			if (_ownerEntityId > -1)
				throw new EcsException(this, $"Trying to add another singleton component {typeof(T)} to entity {_ownerEntityId}");
			
			_ownerEntityId = entityId;
			return ref _component;
		}
		
		public void DelComponent()
		{
			_component = default;
			_ownerEntityId = -1;
		}
	}
}