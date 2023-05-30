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
	}
}