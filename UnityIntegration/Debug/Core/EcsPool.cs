using System;

namespace Secs
{
    public sealed partial class EcsPool<T> : IDisposable where T : struct, IEcsComponent
    {
        internal object GetItem(int entityId)
        {
            return _componentsBuffer[entityId];
        }

        internal void ReplaceComponent(int entityId, T newValue)
        {
            _componentsBuffer[entityId] = newValue;
        }

        internal void Add(int entity, T component)
        {
            ref var cmp = ref AddComponent(entity);
            cmp = component;
        }
    }
}