using System;
using System.Collections.Generic;
using UnityEngine;

namespace Secs
{
	public sealed class EcsWorld
	{
		private int _lastEntityId = -1;

		private readonly HashSet<int> _aliveEntities;
		private readonly Stack<int> _deadEntities;

		private readonly List<EcsEntityUpdateOperation> _entityUpdateOperations;

		private readonly Dictionary<int, EcsTypeMask> _entitiesComponents;
		private readonly Dictionary<int, object> _pools;
		private readonly Dictionary<EcsFilterMask, EcsFilter> _filters;
		
		internal readonly EcsConfig config;
		
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
			_filters = new Dictionary<EcsFilterMask, EcsFilter>(config.world.initialAllocatedFilters);
		}

		internal bool IsEntityDead(int entityId)
		{
			return _deadEntities.Contains(entityId);
		}

		internal EcsTypeMask GetEntityComponentsTypeMask(in int entityId)
		{
			return _entitiesComponents[entityId];
		}

		internal void RegisterComponentAddedOperation<T>(in int entityId) where T : struct
		{
			_entityUpdateOperations.Add(new EcsEntityUpdateOperation
			{
				operationType = EcsEntityUpdateOperation.EcsEntityOperationType.ComponentAdded,
				entityId = entityId,
				componentType = typeof(T)
			});
		}
		
		internal void RegisterComponentDeletedOperation<T>(in int entityId) where T : struct
		{
			_entityUpdateOperations.Add(new EcsEntityUpdateOperation
			{
				operationType = EcsEntityUpdateOperation.EcsEntityOperationType.ComponentAdded,
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

		public EcsPool<T> GetPool<T>() where T : struct
		{
			int typeIndex = EcsTypeIndexUtility.GetIndexOfType(typeof(T));

			if (_pools.ContainsKey(typeIndex))
				return (EcsPool<T>)_pools[typeIndex];
			
			EcsLogger.HeapAlloc<EcsPool<T>>();
			var pool = new EcsPool<T>(config.pool.initialAllocatedComponents, this); 
			_pools.Add(typeIndex, pool);

			return pool;
		}

		public ref T GetComponent<T>(in int entityId) where T : struct
		{
			return ref GetPool<T>().GetComponent(entityId);
		}
		
		public ref T AddComponent<T>(in int entityId) where T : struct
		{
			return ref GetPool<T>().AddComponent(entityId);
		}
		
		public void DelComponent<T>(in int entityId) where T : struct
		{
			GetPool<T>().DelComponent(entityId);	
		}

		public bool HasComponent<T>(in int entityId) where T : struct
		{
			return GetPool<T>().HasComponent(entityId);	
		}

		public EcsFilter GetFilter(EcsFilterMask ecsFilterMask)
		{
			if(ecsFilterMask == null)
				throw new ArgumentNullException(nameof(ecsFilterMask));
			
			if(_filters.ContainsKey(ecsFilterMask))
				return _filters[ecsFilterMask];

			var filter = new EcsFilter(this, ecsFilterMask);
			_filters.Add(ecsFilterMask, filter);

			return filter;
		}
	}
}