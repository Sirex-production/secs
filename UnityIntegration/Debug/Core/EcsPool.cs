using System;

namespace Secs
{
    public sealed partial class EcsPool<T> : IDisposable where T : struct, IEcsComponent
    {
        internal object GetItem(int entityId)
        {
            return _denseComponents[_sparse[entityId] - 1];
        }

        internal void ReplaceComponent(int entityId, T newValue)
        {
            _denseComponents[_sparse[entityId] - 1] = newValue;
        }

        internal void Add(int entity, T component)
        {
            ref var cmp = ref AddComponent(entity);
            cmp = component;
        }
    }
}
