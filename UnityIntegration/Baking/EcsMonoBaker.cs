using System;
using UnityEngine;

namespace Secs
{
	/// <summary>
	/// Base class for defining baking logic for ECS
	/// </summary>
	public abstract class EcsMonoBaker : MonoBehaviour
	{
		[Header("ECS baking properties")]
		[SerializeField] internal string worldId;
		[SerializeField] internal bool createEntityReference = false;
		[SerializeField] internal BakingProperties bakingProperties = BakingProperties.BakeAndDestroyBaker;
		
		/// <summary>
		/// Is fires when world baking is requested on particular scene. Override this method to define your own baking logic 
		/// </summary>
		/// <param name="world">World to which entity belongs</param>
		/// <param name="entityId">Created entity that should be used for baking</param>
		protected internal abstract void Bake(EcsWorld world, int entityId);

		[Serializable]
		internal enum BakingProperties
		{
			BakeAndDestroyBaker = 0,
			BakeAndDestroyGameObject = 1,
			BakeAndKeep = 2,
			DontBake = 3
		}
	}
}