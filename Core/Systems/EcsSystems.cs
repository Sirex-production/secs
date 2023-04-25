using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed class EcsSystems
	{
		private readonly EcsWorld _world;
		private readonly List<IEcsInitSystem> _initSystems;
		private readonly List<IEcsRunSystem> _runSystems;
		private readonly List<IEcsDisposeSystem> _disposeSystems;

		public EcsSystems(EcsWorld world)
		{
			_world = world;
			_initSystems = new List<IEcsInitSystem>();
			_runSystems = new List<IEcsRunSystem>();
			_disposeSystems = new List<IEcsDisposeSystem>();
		}

		public EcsSystems Add(IEcsSystem ecsSystem)
		{
			if(ecsSystem is IEcsInitSystem initSystem) 
				_initSystems.Add(initSystem);

			if(ecsSystem is IEcsRunSystem runSystem) 
				_runSystems.Add(runSystem);
			
			if(ecsSystem is IEcsDisposeSystem destroySystem)
				_disposeSystems.Add(destroySystem);

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireInitSystems()
		{
			foreach (var initSystem in _initSystems) 
				initSystem.OnInit();
			
			_world.UpdateFilters();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireRunSystems()
		{
			foreach (var runSystem in _runSystems) 
				runSystem.OnRun();
			
			_world.UpdateFilters();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireDisposeSystems()
		{
			foreach (var disposeSystem in _disposeSystems) 
				disposeSystem.OnDispose();
			
			_world.UpdateFilters();
		}
	}
}