#if TOOLS
using System.Collections.Generic;

namespace Secs.Debug
{
	/// <summary>
	/// Static registry of worlds observed by the SECS ECS Inspector tool
	/// </summary>
	public static class EcsWorldRegistry
	{
		private static readonly Dictionary<EcsWorld, List<EcsSystems>> RegisteredWorlds = new();

		/// <summary>
		/// Worlds and their systems that are currently observed by the inspector tool
		/// </summary>
		public static IReadOnlyDictionary<EcsWorld, List<EcsSystems>> Worlds => RegisteredWorlds;

		internal static void Register(EcsWorld world, EcsSystems systems)
		{
			if(!RegisteredWorlds.TryGetValue(world, out var registeredSystems))
			{
				registeredSystems = new List<EcsSystems>();
				RegisteredWorlds.Add(world, registeredSystems);
			}

			if(!registeredSystems.Contains(systems))
				registeredSystems.Add(systems);
		}

		internal static void Release(EcsWorld world, EcsSystems systems)
		{
			if(!RegisteredWorlds.TryGetValue(world, out var registeredSystems))
				return;

			registeredSystems.Remove(systems);

			if(registeredSystems.Count == 0)
				RegisteredWorlds.Remove(world);
		}

		internal static void Clear()
		{
			RegisteredWorlds.Clear();
		}
	}
}
#endif
