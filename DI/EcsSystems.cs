﻿using System.Linq;

namespace Secs
{
	public sealed partial class EcsSystems
	{
		public void Inject()
		{
			var allSystems = _initSystems
				.Cast<IEcsSystem>()
				.Concat(_runSystems)
				.Concat(_disposeSystems);
			
			foreach(var ecsSystem in allSystems) 
				_world.Inject(ecsSystem);
		}
	}
}