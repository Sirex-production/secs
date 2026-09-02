using System.Collections.Generic;
#if TOOLS
using Secs.Debug;
#endif

namespace Secs
{
	public sealed partial class EcsSystems
	{
#if TOOLS
		internal List<IEcsSystem> AllSystems => _allSystems;
#endif

		/// <summary>
		/// Attaches ECS inspector observer to this systems. Observed worlds will be visible in the SECS ECS Inspector tool.
		/// Automatically released when this systems' FireDisposeSystems() runs
		/// </summary>
		public void AttachObserver()
		{
#if TOOLS
			EcsWorldRegistry.Register(_world, this);
			OnDisposeFired += ReleaseObserver;
#endif
		}

		/// <summary>
		/// Releases ECS inspector observer from this systems
		/// </summary>
		public void ReleaseObserver()
		{
#if TOOLS
			OnDisposeFired -= ReleaseObserver;
			EcsWorldRegistry.Release(_world, this);
#endif
		}
	}
}
