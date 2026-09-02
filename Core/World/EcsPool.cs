using System;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsPool<T> where T : struct, IEcsComponent
	{
		private readonly EcsWorld _world;

		private T[] _denseComponents;
		private int[] _denseEntityIds;
		private int[] _sparse;
		private int _count;

		internal int Count => _count;

		public EcsPool(int capacity, EcsWorld world)
		{
			_world = world ?? throw new EcsException(this, "World can't be null");
			_denseComponents = new T[capacity];
			_denseEntityIds = new int[capacity];
			_sparse = new int[capacity];

			_world.OnEntityDeleted += OnEntityDeleted;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			_world.OnEntityDeleted -= OnEntityDeleted;
		}

		private void OnEntityDeleted(int entityId)
		{
			if (!HasComponentSparse(entityId))
				return;

			RemoveFromDense(entityId);
			_world.GetEntityComponentsTypeMask(entityId).RemoveType<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool HasComponentSparse(in int entityId)
		{
			return entityId < _sparse.Length && _sparse[entityId] != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GrowSparse(int minSize)
		{
			int resizeSize = Math.Max(_sparse.Length * 2, minSize + 1);
			Array.Resize(ref _sparse, resizeSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GrowDense()
		{
			int resizeSize = Math.Max(_denseComponents.Length * 2, 1);
			Array.Resize(ref _denseComponents, resizeSize);
			Array.Resize(ref _denseEntityIds, resizeSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveFromDense(in int entityId)
		{
			int denseIndex = _sparse[entityId] - 1;
			int lastIndex = _count - 1;

			if (denseIndex != lastIndex)
			{
				int movedEntityId = _denseEntityIds[lastIndex];
				_denseComponents[denseIndex] = _denseComponents[lastIndex];
				_denseEntityIds[denseIndex] = movedEntityId;
				_sparse[movedEntityId] = denseIndex + 1;
			}

			_denseComponents[lastIndex] = default;
			_sparse[entityId] = 0;
			_count--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(in int entityId)
		{
			if(_world.IsEntityDead(entityId))
				throw new EcsException(this, $"Trying to manipulate with dead entity {entityId}");

			if (!HasComponentSparse(entityId))
				throw new EcsException(this, $"Trying to get component that entity {entityId} does not have");

			return ref _denseComponents[_sparse[entityId] - 1];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEcsComponent GetCopy(in int entityId)
		{
			return Get(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(in int entityId, in T cmp)
		{
			ref var cmpRef = ref Get(entityId);
			cmpRef = cmp;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(in int entityId, IEcsComponent cmp)
		{
			if(cmp is not T cmpT)
				throw new EcsException(this, $"Trying to set component of type {cmp.GetType()} to pool of type {typeof(T)}");

			Set(entityId, cmpT);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Add(int entityId)
		{
			if(_world.IsEntityDead(entityId))
				throw new EcsException(this, $"Trying to manipulate with dead entity {entityId}");

			if (HasComponentSparse(entityId))
				throw new EcsException(this, $"Trying to add component that entity {entityId} already have");

			if (entityId >= _sparse.Length)
				GrowSparse(entityId);

			if (_count == _denseComponents.Length)
				GrowDense();

			_denseEntityIds[_count] = entityId;
			_sparse[entityId] = ++_count;

			_world.GetEntityComponentsTypeMask(entityId).AddType<T>();
			_world.RegisterAddedComponent<T>(entityId);

			//Dense index is resolved again since listeners of RegisterAddedComponent may have moved the component
			return ref _denseComponents[_sparse[entityId] - 1];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(in int entityId, IEcsComponent cmp)
		{
			if(cmp is not T cmpT)
				throw new EcsException(this, $"Trying to add component of type {cmp.GetType()} to pool of type {typeof(T)}");

			Add(entityId) = cmpT;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Del(in int entityId)
		{
			if(_world.IsEntityDead(entityId))
				throw new EcsException(this, $"Trying to manipulate with dead entity {entityId}");

			if (!HasComponentSparse(entityId))
				throw new EcsException(this, $"Trying to delete component that entity {entityId} does not have");

			RemoveFromDense(entityId);
			_world.GetEntityComponentsTypeMask(entityId).RemoveType<T>();
			_world.RegisterDeletedComponent<T>(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(in int entityId)
		{
			if(!_world.ContainsEntity(entityId))
				throw new EcsException(this, $"Trying to manipulate non existing entity {entityId}");

			return HasComponentSparse(entityId);
		}
	}
}
