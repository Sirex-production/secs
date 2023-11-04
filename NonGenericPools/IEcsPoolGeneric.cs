namespace Secs
{
	public interface IEcsPoolGeneric<T>
	{
		ref T GetComponent(in int entityId);
		void SetComponent(in int entityId, in T cmp);
		ref T AddComponent(in int entityId);
	}
}