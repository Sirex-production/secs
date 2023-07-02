using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsSystems
	{
		private readonly EcsWorld _world;
		private readonly List<IEcsSystem> _allSystems = new();

		private event Action OnInitFired;
		private event Action OnRunFired;
		private event Action OnDisposeFired;

		private Dictionary<IEcsReactiveSystem, EcsFilter> _reactiveFilters = new();
		private Dictionary<Type, List<IEcsReactiveSystem>> _addedComponentTypesReactiveSystems  = new();
		private Dictionary<Type, List<IEcsReactiveSystem>> _removedComponentTypesReactiveSystems  = new();
		
		public EcsSystems(EcsWorld world)
		{
			_world = world;
			
			_world.OnComponentAddedToEntity += FireAddedComponentReactiveSystems;
			_world.OnComponentDeletedFromEntity += FireRemoveComponentReactiveSystems;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EcsSystems Add(IEcsSystem ecsSystem)
		{
			_allSystems.Add(ecsSystem);

			if(ecsSystem is IEcsInitSystem initSystem) 
				OnInitFired += initSystem.OnInit;

			if(ecsSystem is IEcsRunSystem runSystem) 
				OnRunFired += runSystem.OnRun;

			if(ecsSystem is IEcsDisposeSystem disposeSystems) 
				OnDisposeFired += disposeSystems.OnDispose;


			if (ecsSystem is IEcsReactiveSystem reactiveSystem)
			{
				var state = reactiveSystem.ObserveOnState();
				var type = reactiveSystem.ObserveOnType();
				
				_reactiveFilters.Add(reactiveSystem, reactiveSystem.ObserveFilter(_world));

				if (state is IEcsReactiveSystem.ComponentReactiveState.ComponentAdded
				    or IEcsReactiveSystem.ComponentReactiveState.ComponentAddedOrRemoved)
				{
					if (!_addedComponentTypesReactiveSystems.ContainsKey(type))
						_addedComponentTypesReactiveSystems.Add(type, new List<IEcsReactiveSystem>());
				
					_addedComponentTypesReactiveSystems[type].Add(reactiveSystem);
				}
				
				if (state is IEcsReactiveSystem.ComponentReactiveState.ComponentRemoved
				    or IEcsReactiveSystem.ComponentReactiveState.ComponentAddedOrRemoved)
				{
					if (!_removedComponentTypesReactiveSystems.ContainsKey(type))
						_removedComponentTypesReactiveSystems.Add(type, new List<IEcsReactiveSystem>());
				
					_removedComponentTypesReactiveSystems[type].Add(reactiveSystem);
				}
			}
			
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireInitSystems()
		{
			OnInitFired?.Invoke();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireRunSystems()
		{
			OnRunFired?.Invoke();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireDisposeSystems()
		{
			OnDisposeFired?.Invoke();
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireAddedComponentReactiveSystems(int entityId, Type type)
		{
			if (!_addedComponentTypesReactiveSystems.ContainsKey(type))
				return;

			var cashedReactiveSystems = _addedComponentTypesReactiveSystems[type];
			
			foreach (var reactive in cashedReactiveSystems)
			{
				var filter = _reactiveFilters[reactive];
				
				if (filter == null || filter.HasEntity(entityId))
					reactive.OnExecute(entityId);
			
			}
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireRemoveComponentReactiveSystems(int entityId, Type type)
		{
			if (!_removedComponentTypesReactiveSystems.ContainsKey(type))
				return;

			var cashedReactiveSystems = _removedComponentTypesReactiveSystems[type];
			
			foreach (var reactive in cashedReactiveSystems)
			{
				var filter = _reactiveFilters[reactive];
				
				if (filter == null || filter.HasEntity(entityId))
					reactive.OnExecute(entityId);
			
			}
		}
	}
}