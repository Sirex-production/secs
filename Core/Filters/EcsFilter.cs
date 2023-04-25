using System;
using System.Collections;
using System.Collections.Generic;

namespace Secs
{
	public sealed class EcsFilter : IEnumerable<int>, IDisposable
	{
		private readonly EcsWorld _world;
		private readonly EcsFilterMask _filterMask;
		private readonly HashSet<int> _entities;

		public EcsFilter(EcsWorld world, EcsFilterMask filterMask)
		{
			_world = world ?? throw new ArgumentNullException(nameof(world));
			_filterMask = filterMask;
			_entities = new HashSet<int>(world.config.filter.initialAllocatedEntities);
			
			_world.OnEntityDeleted += OnEntityDeleted;
			_world.OnComponentAddedToEntity += OnComponentAddedToEntity;
			_world.OnComponentDeletedFromEntity += OnComponentDeletedFromEntity;
		}

		public void Dispose()
		{
			_world.OnEntityDeleted -= OnEntityDeleted;
			_world.OnComponentAddedToEntity -= OnComponentAddedToEntity;
			_world.OnComponentDeletedFromEntity -= OnComponentDeletedFromEntity;
		}
		
		private void OnEntityDeleted(int entityId)
		{
			_entities.Remove(entityId);
		}

		private void OnComponentAddedToEntity(int entityId, Type componentType)
		{
			if(_filterMask.IsExcluded(componentType))
			{
				_entities.Remove(entityId);
				return;
			}

			if(!_filterMask.IsIncluded(componentType))
				return;

			var entityComponents = _world.GetEntityComponentsTypeMask(entityId);
			
			if(_filterMask.IsSameAsIncludeMask(entityComponents))
				_entities.Add(entityId);
		}

		private void OnComponentDeletedFromEntity(int entityId, Type componentType)
		{
			if(_filterMask.IsIncluded(componentType))
			{
				_entities.Remove(entityId);
				return;
			}

			if(!_filterMask.IsExcluded(componentType))
				return;
		
			var entityComponents = _world.GetEntityComponentsTypeMask(entityId);

			if(_filterMask.IsSameAsIncludeMask(entityComponents))
				_entities.Add(entityId);
		}
#region Enumeration
		public IEnumerator<int> GetEnumerator()
		{
			return _entities.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
#endregion
	}
}