using System;

namespace Secs
{
    public abstract class EcsReactiveSystem : IEcsSystem /*: IEcsReactiveSystem*/
    {   
        private EcsWorld _ecsWorld;
        private IEcsReactiveSystem.ComponentReactiveState _componentReactiveState;
        private EcsFilter _filter;
        private Type _observeType;
        private bool _activated; 
        internal void Init(EcsWorld ecsWorld)
        {
            _ecsWorld = ecsWorld;
            _componentReactiveState = ObserveOn();
            _filter = CreateFilter(in _ecsWorld);
            _observeType = ObserveType();
            Activate();
        }
        
        private void OnComponentAddedToEntity(int i, Type type)
        {
            if(type != _observeType)
                return;

            if (!_filter.HasEntity(i))
                return;

            OnExecute();
        }
        
        private void OnComponentDeletedFromEntity(int i, Type type)
        {
            if(type != _observeType)
                return;

            if (!_filter.HasEntity(i))
                return;

            OnExecute();
        }
        
        
        public void Clear()
        {
         
        }
        public void Activate()
        {
            if(_activated)
                return;
            
            if(_componentReactiveState is IEcsReactiveSystem.ComponentReactiveState.ComponentAdded or IEcsReactiveSystem.ComponentReactiveState.Both )
                _ecsWorld.OnComponentAddedToEntity += OnComponentAddedToEntity;
            
            if(_componentReactiveState is IEcsReactiveSystem.ComponentReactiveState.ComponentRemoved or IEcsReactiveSystem.ComponentReactiveState.Both )
                _ecsWorld.OnComponentDeletedFromEntity += OnComponentDeletedFromEntity;

            _activated = true;
        }

        public void Deactivate()
        {
            if(!_activated)
                return;
            
            if(_componentReactiveState is IEcsReactiveSystem.ComponentReactiveState.ComponentAdded or IEcsReactiveSystem.ComponentReactiveState.Both )
                _ecsWorld.OnComponentAddedToEntity -= OnComponentAddedToEntity;
            
            if(_componentReactiveState is IEcsReactiveSystem.ComponentReactiveState.ComponentRemoved or IEcsReactiveSystem.ComponentReactiveState.Both )
                _ecsWorld.OnComponentDeletedFromEntity -= OnComponentDeletedFromEntity;

            _activated = false;
        }
        
        protected abstract EcsFilter CreateFilter(in EcsWorld ecsWorld);
        protected abstract Type ObserveType();
        public abstract void OnExecute();
        protected abstract IEcsReactiveSystem.ComponentReactiveState ObserveOn();
        

        /*~EcsReactiveSystem() => this.Deactivate();*/
    }
}