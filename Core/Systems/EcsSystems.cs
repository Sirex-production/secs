using System.Runtime.CompilerServices;
using Unity.Plastic.Newtonsoft.Json.Serialization;

namespace Secs
{
	public sealed class EcsSystems
	{
		private readonly EcsWorld _world;
		private event Action OnInitSystemsFired; 
		private event Action OnRunSystemsFired; 
		private event Action OnDisposeSystemsFired;

		public EcsSystems(EcsWorld world)
		{
			_world = world;
		}

		public EcsSystems Add(IEcsSystem ecsSystem)
		{
			if(ecsSystem is IEcsInitSystem initSystem) 
				OnInitSystemsFired += initSystem.OnInit;

			if(ecsSystem is IEcsRunSystem runSystem) 
				OnRunSystemsFired += runSystem.OnRun;
			
			if(ecsSystem is IEcsDisposeSystem disposeSystem)
				OnDisposeSystemsFired += disposeSystem.OnDispose;

			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireInitSystems()
		{
			OnInitSystemsFired?.Invoke();
			_world.UpdateFilters();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireRunSystems()
		{
			OnRunSystemsFired?.Invoke();
			_world.UpdateFilters();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FireDisposeSystems()
		{
			OnDisposeSystemsFired?.Invoke();
			_world.UpdateFilters();
		}
	}
}