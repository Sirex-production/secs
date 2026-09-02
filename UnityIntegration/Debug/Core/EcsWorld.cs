
namespace Secs
{
    public sealed partial class EcsWorld
    {
        internal T GetItem<T>(int entity) where T : struct, IEcsComponent
        {
           return (T) GetPool<T>().GetItem(entity);
        }
        
        internal T GetSingletonItem<T>() where T : struct, IEcsSingletonComponent
        {
            return GetSingletonPool<T>().Component;
        }
        
        internal void ReplaceItem<T>(int entity, T newValue) where T : struct, IEcsComponent
        {
            GetPool<T>().ReplaceItem(entity, newValue);
        }
        
        internal void AddItem<T>(int entity, T newValue) where T : struct, IEcsComponent
        {
            GetPool<T>().AddItem(entity, newValue);
        }

        internal void DelItem<T>(int i) where T : struct, IEcsComponent
        {
            GetPool<T>().Del(i);
        }

        internal bool IsSame<T>(int i, ref object d) where T : struct, IEcsComponent
        {
            return Equals(GetPool<T>().Get(i), d);
        }
    }
}