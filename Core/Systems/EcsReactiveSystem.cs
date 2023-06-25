using System;
using System.Runtime.CompilerServices;

namespace Secs
{
    public abstract class EcsReactiveSystem : IEcsReactiveSystem
    {
        protected enum ComponentReactiveState
        {
            ComponentAdded,
            ComponentRemoved,
            ComponentAddedOrRemoved
        }
        
        private EcsWorld _ecsWorld;
        private ComponentReactiveState _componentReactiveState;
        private EcsFilter _filter;
        private Type _observeType;
        private bool _activated;
        
        internal void Init(EcsWorld ecsWorld)
        {
            _ecsWorld = ecsWorld;
            _componentReactiveState = ObserveOnState();
            _filter = CreateFilter(in _ecsWorld);
            _observeType = ObserveOnType();
            
            Activate();
        }
        
        private void OnComponentAddedToEntity(int entityId, Type type) => OnComponentAction(entityId, type);
        private void OnComponentDeletedFromEntity(int entityId, Type type) => OnComponentAction(entityId, type);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnComponentAction(int entityId, Type type)
        {
            if(type != _observeType)
                return;

            if (_filter == null)
            {
                OnExecute(in entityId);
                return;
            }
            
            if (!_filter.HasEntity(entityId))
                return;

            OnExecute(in entityId);
            
        }
        
        protected virtual EcsFilter CreateFilter(in EcsWorld ecsWorld) => null;
        protected abstract Type ObserveOnType();
        protected abstract void OnExecute(in int entityId);
        protected abstract ComponentReactiveState ObserveOnState();
        
        public void Activate()
        {
            if(_activated)
                return;
            
            if(_ecsWorld == null)
                return;
            
            if(_componentReactiveState is ComponentReactiveState.ComponentAdded or ComponentReactiveState.ComponentAddedOrRemoved )
                _ecsWorld.OnComponentAddedToEntity += OnComponentAddedToEntity;
            
            if(_componentReactiveState is ComponentReactiveState.ComponentRemoved or ComponentReactiveState.ComponentAddedOrRemoved )
                _ecsWorld.OnComponentDeletedFromEntity += OnComponentDeletedFromEntity;

            _activated = true;
        }

        public void Deactivate()
        {
            if(!_activated)
                return;
            
            if(_ecsWorld == null)
                return;
            
            if(_componentReactiveState is ComponentReactiveState.ComponentAdded or ComponentReactiveState.ComponentAddedOrRemoved )
                _ecsWorld.OnComponentAddedToEntity -= OnComponentAddedToEntity;
            
            if(_componentReactiveState is ComponentReactiveState.ComponentRemoved or ComponentReactiveState.ComponentAddedOrRemoved )
                _ecsWorld.OnComponentDeletedFromEntity -= OnComponentDeletedFromEntity;

            _activated = false;
        }
        
        
        ~EcsReactiveSystem() => this.Deactivate();
    }
}