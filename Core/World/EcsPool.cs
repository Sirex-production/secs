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
			_world = world ?? throw new EcsException(this, "World can't be null");
			_componentsBuffer = new T[capacity];

			_world.OnEntityDeleted += OnEntityDeleted;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			int resizeSize = Math.Max(_componentsBuffer.Length * 2, minSize + 1);
			Array.Resize(ref _componentsBuffer, resizeSize);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetComponent(in int entityId)
		{
			if(_world.IsEntityDead(entityId))
				throw new EcsException(this, $"Trying to manipulate with dead entity {entityId}");
			
			if (!HasComponent(entityId))
				throw new EcsException(this, $"Trying to get component that entity {entityId} does not have");

			return ref _componentsBuffer[entityId];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T AddComponent(in int entityId)
		{
			if(_world.IsEntityDead(entityId))
				throw new EcsException(this, $"Trying to manipulate with dead entity {entityId}");
			
			if (HasComponent(entityId))
				throw new EcsException(this, $"Trying to add component that entity {entityId} already have");
			
			if (entityId >= _componentsBuffer.Length)
				GrowBuffer(entityId);
			
			_world.GetEntityComponentsTypeMask(entityId).AddType<T>();
			_world.RegisterAddedComponent<T>(entityId);

			return ref _componentsBuffer[entityId];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DelComponent(in int entityId)
		{
			if(_world.IsEntityDead(entityId))
				throw new EcsException(this, $"Trying to manipulate with dead entity {entityId}");
			
			if (!HasComponent(entityId))
				throw new EcsException(this, $"Trying to delete component that entity {entityId} does not have");
			
			_componentsBuffer[entityId] = default;
			_world.GetEntityComponentsTypeMask(entityId).RemoveType<T>();
			_world.RegisterDeletedComponent<T>(entityId);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasComponent(in int entityId)
		{
			return _world.GetEntityComponentsTypeMask(entityId).ContainsType<T>();
		}
	}
}