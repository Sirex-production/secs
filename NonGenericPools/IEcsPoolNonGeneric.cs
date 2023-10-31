namespace Secs
{
	public interface IEcsPoolNonGeneric
	{
		IEcsComponent GetComponentCopyVirtual(in int entityId);
		void SetComponentVirtual(in int entityId, IEcsComponent cmp);
		void AddComponentVirtual(in int entityId, IEcsComponent cmp);
		void DelComponentVirtual(in int entityId);
		bool HasComponentVirtual(in int entityId);
	}
}