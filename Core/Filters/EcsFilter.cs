using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsFilter : IEnumerable<int>, IDisposable
	{
		private readonly EcsWorld _world;
		private readonly EcsMatcher _matcher;
		
		private readonly HashSet<int> _entities;
		
		private int[] _iterateEntities;
		private bool _isEntitiesSetModified = false;
	
		internal EcsMatcher Matcher
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _matcher;
		}

		public int EntitiesCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _entities.Count;
		}

		public EcsFilter(EcsWorld world, EcsMatcher matcher)
		{
			_world = world ?? throw new ArgumentNullException(nameof(world));
			_matcher = matcher;
			_entities = new HashSet<int>(world.config.filter.initialAllocatedEntities);
			_iterateEntities = ArrayPool<int>.Shared.Rent(world.config.filter.initialAllocatedEntities);
			
			_world.OnEntityDeleted += OnEntityDeleted;
			_world.OnComponentAddedToEntity += OnComponentAddedToEntity;
			_world.OnComponentDeletedFromEntity += OnComponentDeletedFromEntity;

			FetchAliveEntities();
		}

		public void Dispose()
		{
			_world.OnEntityDeleted -= OnEntityDeleted;
			_world.OnComponentAddedToEntity -= OnComponentAddedToEntity;
			_world.OnComponentDeletedFromEntity -= OnComponentDeletedFromEntity;
		}
		
		private void OnEntityDeleted(int entityId)
		{
			if(_world.IsEntityDead(entityId))
			{
				_entities.Remove(entityId);
				_isEntitiesSetModified = true;
			}
		}

		private void OnComponentAddedToEntity(int entityId, Type componentType)
		{
			//If excluded type was added to the entity, remove it from the filter
			if(_entities.Contains(entityId) && _matcher.IsExcluded(componentType))
			{
				_entities.Remove(entityId);
				_isEntitiesSetModified = true;
				return;
			}
			
			//If added component is not included to the filter, do nothing - further actions are performed only if included type was added to the entity
			if(!_matcher.IsIncluded(componentType))
				return;

			if(EntityMatchesToTheFilterMask(entityId))
			{
				_entities.Add(entityId);
				_isEntitiesSetModified = true;
			}
		}

		private void OnComponentDeletedFromEntity(int entityId, Type componentType)
		{
			//If included type was deleted from the entity, remove it from the filter
			if(_entities.Contains(entityId) && _matcher.IsIncluded(componentType))
			{
				_entities.Remove(entityId);
				_isEntitiesSetModified = true;
				return;
			}

			//If removed type is not excluded by the filter, do nothing - further actions are performed only if excluded type was removed from the entity
			if(!_matcher.IsExcluded(componentType))
				return;
			
			if(EntityMatchesToTheFilterMask(entityId))
			{
				_entities.Add(entityId);
				_isEntitiesSetModified = true;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool EntityMatchesToTheFilterMask(in int entityId)
		{
			var filterIncludeMask = _matcher.includeTypeMask;
			var filterExcludeMask = _matcher.excludeTypeMask;
			var entityComponentsTypeMask = _world.GetEntityComponentsTypeMask(entityId);

			if(!filterIncludeMask.Includes(entityComponentsTypeMask))
				return false;
			
			return filterExcludeMask == null || !filterExcludeMask.HasCommonTypeWith(entityComponentsTypeMask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FetchAliveEntities()
		{
			foreach(int entityId in _world.AliveEntities)
			{
				if(EntityMatchesToTheFilterMask(entityId))
				{
					_entities.Add(entityId);
					_isEntitiesSetModified = true;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasEntity(in int entity)
		{
			return _entities.Contains(entity);
		}
		
#region Enumeration
		public IEnumerator<int> GetEnumerator()
		{
			if(_entities.Count < 1)
				return new Enumerator(null, 0);

			if(_isEntitiesSetModified)
			{
				ArrayPool<int>.Shared.Return(_iterateEntities);
				_iterateEntities = ArrayPool<int>.Shared.Rent(_entities.Count);
				_entities.CopyTo(_iterateEntities);
			}

			return new Enumerator(_iterateEntities, _entities.Count);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public struct Enumerator : IEnumerator<int>
		{
			private int[] _entities;
			private int _currentIndex;
			private readonly int _entitiesCount;

			int IEnumerator<int>.Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _entities[_currentIndex];
			}
			
			public object Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _entities[_currentIndex];
			}

			public Enumerator(int[] entities, int entitiesCount)
			{
				_entities = entities;
				_entitiesCount = entitiesCount;
				_currentIndex = -1;
			}
			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				return ++_currentIndex < _entitiesCount;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Reset()
			{
				_currentIndex = -1;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
				if(_entities != null)
					ArrayPool<int>.Shared.Return(_entities);
				
				_entities = null;
			}
		}
#endregion
	}
}