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
		
		public EcsSystems(EcsWorld world)
		{
			_world = world;
		}

		public EcsSystems Add(IEcsSystem ecsSystem)
		{
			_allSystems.Add(ecsSystem);

			if(ecsSystem is IEcsInitSystem initSystem)
				OnInitFired += initSystem.OnOnInit;

			if(ecsSystem is IEcsRunSystem runSystem)
				OnRunFired += runSystem.OnOnRun;

			if(ecsSystem is IEcsDisposeSystem disposeSystems)
				OnDisposeFired += disposeSystems.OnOnDispose;

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireInitSystems()
		{
			OnInitFired?.Invoke();
			_world.UpdateFilters();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireRunSystems()
		{
			OnRunFired?.Invoke();
			_world.UpdateFilters();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireDisposeSystems()
		{
			OnDisposeFired?.Invoke();
			_world.UpdateFilters();
		}
	}
}