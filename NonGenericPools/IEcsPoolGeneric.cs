namespace Secs
{
	public interface IEcsPoolGeneric<T>
	{
		ref T Get(in int entityId);
		void Set(in int entityId, in T cmp);
		ref T Add(in int entityId);
	}
}