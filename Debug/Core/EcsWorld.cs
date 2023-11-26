
namespace Secs
{
    public sealed partial class EcsWorld
    {
        internal T GetItem<T>(int entity) where T : struct, IEcsComponent
        {
           return (T) GetPool<T>().GetItem(entity);
        }
        
        
        internal void ReplaceItem<T>(int entity, T newValue) where T : struct, IEcsComponent
        {
            GetPool<T>().ReplaceComponent(entity, newValue);
        }
        
        internal void AddItem<T>(int entity, T newValue) where T : struct, IEcsComponent
        {
            GetPool<T>().Add(entity, newValue);
        }

        internal void DeleteComponent<T>(int i) where T : struct, IEcsComponent
        {
            GetPool<T>().DelComponent(i);
        }

        internal bool IsSame<T>(int i, ref object d) where T : struct, IEcsComponent
        {
            return Equals(GetPool<T>().GetComponent(i), d);
        }
    }
}