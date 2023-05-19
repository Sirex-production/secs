using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		private int _lastEntityId = -1;

		internal readonly EcsConfig config;
		
		private readonly HashSet<int> _aliveEntities;
		private readonly Stack<int> _deadEntities;
		
		private readonly List<EcsEntityUpdateOperation> _entityUpdateOperations;

		private readonly Dictionary<int, EcsTypeMask> _entitiesComponents;
		private readonly Dictionary<int, object> _pools;
		private readonly Dictionary<EcsMatcher, EcsFilter> _filters;

		internal event Action<int> OnEntityCreated;
		internal event Action<int> OnEntityDeleted;
		internal event Action<int, Type> OnComponentAddedToEntity;
		internal event Action<int, Type> OnComponentDeletedFromEntity;

		public EcsWorld() : this(EcsConfig.Default) { }

		public EcsWorld(EcsConfig config)
		{
			this.config = config;

			_aliveEntities = new HashSet<int>(config.world.initialAllocatedEntities);
			_deadEntities = new Stack<int>(config.world.initialAllocatedEntities);
			
			_entityUpdateOperations = new List<EcsEntityUpdateOperation>(config.world.initialAllocatedEntityUpdateOperations);

			_entitiesComponents = new Dictionary<int, EcsTypeMask>(config.world.initialAllocatedEntities);
			_pools = new Dictionary<int, object>(config.world.initialAllocatedPools);
			_filters = new Dictionary<EcsMatcher, EcsFilter>(config.world.initialAllocatedFilters);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsEntityDead(int entityId)
		{
			return _deadEntities.Contains(entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal EcsTypeMask GetEntityComponentsTypeMask(in int entityId)
		{
			return _entitiesComponents[entityId];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void RegisterComponentAddedOperation<T>(in int entityId) where T : struct, IEcsComponent
		{
			_entityUpdateOperations.Add(new EcsEntityUpdateOperation
			{
				operationType = EcsEntityUpdateOperation.EcsEntityOperationType.ComponentAdded,
				entityId = entityId,
				componentType = typeof(T)
			});
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void RegisterComponentDeletedOperation<T>(in int entityId) where T : struct, IEcsComponent
		{
			_entityUpdateOperations.Add(new EcsEntityUpdateOperation
			{
				operationType = EcsEntityUpdateOperation.EcsEntityOperationType.ComponentDeleted,
				entityId = entityId,
				componentType = typeof(T)
			});
		}

		internal void UpdateFilters()
		{
			foreach(var updateOperation in _entityUpdateOperations)
			{
				int entityId = updateOperation.entityId;
				var componentType = updateOperation.componentType;
				
				switch(updateOperation.operationType)
				{
					case EcsEntityUpdateOperation.EcsEntityOperationType.ComponentAdded:
						OnComponentAddedToEntity?.Invoke(entityId, componentType);
						break;
					case EcsEntityUpdateOperation.EcsEntityOperationType.ComponentDeleted:
						OnComponentDeletedFromEntity?.Invoke(entityId, componentType);
						break;
					case EcsEntityUpdateOperation.EcsEntityOperationType.EntityDeleted:
						OnEntityDeleted?.Invoke(entityId);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			
			_entityUpdateOperations.Clear();
		}

		public int NewEntity()
		{
			if (_deadEntities.TryPop(out int entityId))
				return entityId;

			_lastEntityId++;
			
			_aliveEntities.Add(_lastEntityId);
			_entitiesComponents.Add(_lastEntityId, new EcsTypeMask());
			OnEntityCreated?.Invoke(_lastEntityId);
			return _lastEntityId;
		}

		public void DelEntity(in int entityId)
		{
			if (!_aliveEntities.Contains(entityId))
				throw new ArgumentException("Trying to delete existing entity");

			_aliveEntities.Remove(entityId);
			_deadEntities.Push(entityId);
			_entityUpdateOperations.Add(new EcsEntityUpdateOperation
			{
				operationType = EcsEntityUpdateOperation.EcsEntityOperationType.EntityDeleted,
				entityId = entityId,
				componentType = null
			});
		}
		
		public EcsPool<T> GetPool<T>() where T : struct, IEcsComponent
		{
			int typeIndex = EcsTypeIndexUtility.GetIndexOfType(typeof(T));

			if (_pools.ContainsKey(typeIndex))
				return (EcsPool<T>)_pools[typeIndex];
			
			var pool = new EcsPool<T>(config.pool.initialAllocatedComponents, this); 
			_pools.Add(typeIndex, pool);

			return pool;
		}

		public EcsFilter GetFilter(EcsMatcher ecsMatcher)
		{
			if(ecsMatcher == null)
				throw new ArgumentNullException(nameof(ecsMatcher));
			
			if(_filters.ContainsKey(ecsMatcher))
				return _filters[ecsMatcher];

			var filter = new EcsFilter(this, ecsMatcher);
			_filters.Add(ecsMatcher, filter);

			return filter;
		}
	}
}