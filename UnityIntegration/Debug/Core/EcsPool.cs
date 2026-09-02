using System;

namespace Secs
{
    public sealed partial class EcsPool<T> : IDisposable where T : struct, IEcsComponent
    {
        internal object GetItem(int entityId)
        {
            return _denseComponents[_sparse[entityId] - 1];
        }

        internal void ReplaceItem(int entityId, T newValue)
        {
            _denseComponents[_sparse[entityId] - 1] = newValue;
        }

        internal void AddItem(int entity, T component)
        {
            ref var cmp = ref Add(entity);
            cmp = component;
        }
    }
}
