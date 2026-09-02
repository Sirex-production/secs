namespace Secs
{
	public interface IEcsPoolNonGeneric
	{
		IEcsComponent GetCopyVirtual(in int entityId);
		void SetVirtual(in int entityId, IEcsComponent cmp);
		void AddVirtual(in int entityId, IEcsComponent cmp);
		void DelVirtual(in int entityId);
		bool HasVirtual(in int entityId);
	}
}