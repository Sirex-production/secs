using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsPool<T> where T : struct, IEcsComponent
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ref T GetComponentWithNonRefArguments(int entityId)
		{
			if(_world.IsEntityDead(entityId))
				throw new EcsException(this, $"Trying to manipulate with dead entity {entityId}");
			
			if (!HasComponent(entityId))
				throw new EcsException(this, $"Trying to get component that entity {entityId} does not have");

			return ref _componentsBuffer[entityId];
		}

	}
}