using System;
using UnityEngine;

namespace Secs
{
	public abstract class EcsMonoBaker : MonoBehaviour
	{
		[Header("ECS baking properties")]
		[SerializeField] internal string worldId;
		[SerializeField] internal bool createEntityReference = false;
		[SerializeField] internal BakingProperties bakingProperties = BakingProperties.BakeAndDestroyBaker;
		
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