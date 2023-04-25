using System;

namespace Secs
{
	internal struct EcsEntityUpdateOperation
	{
		public EcsEntityOperationType operationType;
		public int entityId;
		public Type componentType;

		public enum EcsEntityOperationType
		{
			ComponentAdded,
			ComponentDeleted,
			EntityDeleted
		}
	}
}