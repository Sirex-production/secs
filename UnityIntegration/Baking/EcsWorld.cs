﻿using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void BakeSpecificBaker(EcsMonoBaker baker)
		{
			if(baker.bakingProperties == EcsMonoBaker.BakingProperties.DontBake)
				return;

			if(string.IsNullOrEmpty(baker.worldId))
			{
				UnityEngine.Debug.LogError($"EcsWorld id is not set for baker of type {baker.GetType().Name}");
				return;
			}
				
			if(baker.worldId != Id)
				return;

			int entityId = NewEntity();
				
			baker.Bake(this, entityId);
			if(baker.createEntityReference)
				baker.LinkEcsEntity(this, entityId);

			if(baker.bakingProperties == EcsMonoBaker.BakingProperties.BakeAndDestroyBaker)
			{
				UnityEngine.Object.Destroy(baker);
				return;
			}
				
			if(baker.bakingProperties == EcsMonoBaker.BakingProperties.BakeAndDestroyGameObject)
				UnityEngine.Object.Destroy(baker.gameObject);
		}
		
		public void BakeAllBakersInScene()
		{
			var bakers = UnityEngine.Object.FindObjectsOfType<EcsMonoBaker>();

			foreach(var baker in bakers) 
				BakeSpecificBaker(baker);
		}
	}
}