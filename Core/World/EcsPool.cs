using System;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsPool<T> : IDisposable where T : struct, IEcsComponent
	{
		private readonly EcsWorld _world;
		
		private T[] _componentsBuffer;

		public EcsPool(int capacity, EcsWorld world)
		{
			_world = world ?? throw new ArgumentNullException(nameof(world));
			_componentsBuffer = new T[capacity];

			_world.OnEntityDeleted += OnEntityDeleted;
		}
		
		public void Dispose()
		{
			_world.OnEntityDeleted -= OnEntityDeleted;
		}

		private void OnEntityDeleted(int entityId)
		{
			if (!HasComponent(entityId))
				return;
				
			_componentsBuffer[entityId] = default;
			_world.GetEntityComponentsTypeMask(entityId).RemoveType<T>();
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GrowBuffer(int minSize)
		{
			int resizeSize = Math.Max(_componentsBuffer.Length * 2, minSize);
			Array.Resize(ref _componentsBuffer, resizeSize);
		}
		
		public ref T GetComponent(in int entityId)
		{
			if(_world.IsEntityDead(entityId))
				throw new ArgumentException("Trying to manipulate with dead entity");
			
			if (!HasComponent(entityId))
				throw new ArgumentException("Trying to get component that entity does not have");

			return ref _componentsBuffer[entityId];
		}

		public ref T AddComponent(in int entityId)
		{
			if(_world.IsEntityDead(entityId))
				throw new ArgumentException("Trying to manipulate with dead entity");
			
			if (HasComponent(entityId))
				throw new ArgumentException("Trying to add component that entity already have");
			
			if (entityId >= _componentsBuffer.Length)
				GrowBuffer(entityId);
			
			_world.GetEntityComponentsTypeMask(entityId).AddType<T>();
			_world.RegisterComponentAddedOperation<T>(entityId);

			return ref _componentsBuffer[entityId];
		}

		public void DelComponent(in int entityId)
		{
			if(_world.IsEntityDead(entityId))
				throw new ArgumentException("Trying to manipulate with dead entity");
			
			if (!HasComponent(entityId))
				throw new ArgumentException("Trying to delete component that entity does not have");
			
			_componentsBuffer[entityId] = default;
			_world.GetEntityComponentsTypeMask(entityId).RemoveType<T>();
			_world.RegisterComponentDeletedOperation<T>(entityId);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasComponent(in int entityId)
		{
			return _world.GetEntityComponentsTypeMask(entityId).ContainsType<T>();
		}
	}
}