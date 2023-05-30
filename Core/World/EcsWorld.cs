using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		internal readonly EcsConfig config;

		private readonly string _id;

		private readonly HashSet<int> _aliveEntities;
		private readonly Stack<int> _deadEntities;

		private readonly Dictionary<int, EcsTypeMask> _entitiesComponents;
		private readonly Dictionary<int, object> _pools;
		private readonly Dictionary<EcsMatcher, EcsFilter> _filters;

		private int _lastEntityId = -1;

		internal event Action<int> OnEntityCreated;
		internal event Action<int> OnEntityDeleted;
		internal event Action<int, Type> OnComponentAddedToEntity;
		internal event Action<int, Type> OnComponentDeletedFromEntity;

		public string Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _id;
		}

		public EcsWorld() : this(EcsConfig.Default)
		{
		}

		public EcsWorld(in string id) : this(EcsConfig.Default, id)
		{
		}

		public EcsWorld(EcsConfig config, in string id = null)
		{
			this.config = config;
			_id = string.IsNullOrWhiteSpace(id) ? this.config.world.defaultWorldId : id;

			_aliveEntities = new HashSet<int>(config.world.initialAllocatedEntities);
			_deadEntities = new Stack<int>(config.world.initialAllocatedEntities);

			_entitiesComponents = new Dictionary<int, EcsTypeMask>(config.world.initialAllocatedEntities);
			_pools = new Dictionary<int, object>(config.world.initialAllocatedPools);
			_filters = new Dictionary<EcsMatcher, EcsFilter>(config.world.initialAllocatedFilters);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal EcsTypeMask GetEntityComponentsTypeMask(in int entityId)
		{
			if(!_entitiesComponents.ContainsKey(entityId))
				throw new EcsException(this, $"Trying to get components type mask of entity {entityId} that does not exist");
			
			return _entitiesComponents[entityId];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void RegisterAddedComponent<T>(in int entityId) where T : struct, IEcsComponent
		{
			OnComponentAddedToEntity?.Invoke(entityId, typeof(T));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void RegisterDeletedComponent<T>(in int entityId) where T : struct, IEcsComponent
		{
			OnComponentDeletedFromEntity?.Invoke(entityId, typeof(T));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsEntityDead(int entityId)
		{
			return _deadEntities.Contains(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int NewEntity()
		{
			bool isEntityReused = _deadEntities.TryPop(out int reusedEntityId);
			int actualEntityId = isEntityReused ? reusedEntityId : ++_lastEntityId;

			if(!isEntityReused)
				_entitiesComponents.Add(actualEntityId, new EcsTypeMask());

			_aliveEntities.Add(actualEntityId);
			OnEntityCreated?.Invoke(actualEntityId);

			return actualEntityId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DelEntity(in int entityId)
		{
			if(!_aliveEntities.Contains(entityId))
				throw new EcsException(this, $"Trying to delete non-existing entity ({entityId})");

			_aliveEntities.Remove(entityId);
			_deadEntities.Push(entityId);
			OnEntityDeleted?.Invoke(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EcsPool<T> GetPool<T>() where T : struct, IEcsComponent
		{
			int typeIndex = EcsTypeIndexUtility.GetIndexOfType(typeof(T));

			if(_pools.ContainsKey(typeIndex))
				return (EcsPool<T>)_pools[typeIndex];

			var pool = new EcsPool<T>(config.pool.initialAllocatedComponents, this);
			_pools.Add(typeIndex, pool);

			return pool;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EcsFilter GetFilter(EcsMatcher ecsMatcher)
		{
			if(ecsMatcher == null)
				throw new EcsException(this, "Trying to get filter with null matcher");

			if(_filters.ContainsKey(ecsMatcher))
				return _filters[ecsMatcher];

			var filter = new EcsFilter(this, ecsMatcher);
			_filters.Add(ecsMatcher, filter);

			return filter;
		}
	}
}