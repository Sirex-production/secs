using System;

namespace Secs
{
    public interface IEcsReactiveSystem : IEcsSystem
    {
        
        public enum ComponentReactiveState
        {
            ComponentAdded,
            ComponentRemoved,
            ComponentAddedOrRemoved
        }

        public EcsFilter ObserveFilter(in EcsWorld ecsWorld);
        public Type ObserveOnType();
        public void OnExecute(in int entityId);

        public ComponentReactiveState ObserveOnState();
        
    }
}