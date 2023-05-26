using System;
using System.Collections;
using System.Collections.Generic;

namespace Secs
{
	public sealed class EcsFilter : IEnumerable<int>, IDisposable
	{
		private readonly EcsWorld _world;
		private readonly EcsMatcher _matcher;
		private readonly HashSet<int> _entities;

		public EcsFilter(EcsWorld world, EcsMatcher matcher)
		{
			_world = world ?? throw new ArgumentNullException(nameof(world));
			_matcher = matcher;
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
			if(_matcher.IsExcluded(componentType))
			{
				_entities.Remove(entityId);
				return;
			}
			
			if(!_matcher.IsIncluded(componentType))
				return;

			var entityComponentsTypeMask = _world.GetEntityComponentsTypeMask(entityId);
			
			if(entityComponentsTypeMask.Includes(_matcher.includeTypeMask))
				_entities.Add(entityId);
		}

		private void OnComponentDeletedFromEntity(int entityId, Type componentType)
		{
			if(_matcher.IsIncluded(componentType))
			{
				_entities.Remove(entityId);
				return;
			}

			if(!_matcher.IsExcluded(componentType))
				return;
		
			var entityComponentsTypeMask = _world.GetEntityComponentsTypeMask(entityId);
			
			if(entityComponentsTypeMask.Includes(_matcher.includeTypeMask))
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