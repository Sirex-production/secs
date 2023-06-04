using Secs;
using UnityEngine;

namespace Secs
{
    public static class EcsPhysics  
    {
        private static EcsWorld _ecsWorld;

        private static EcsPool<OnTriggerEnterReq> _onTriggerEnterEvent;
        private static EcsPool<OnTriggerStayReq> _onTriggerStayEvent;
        private static EcsPool<OnTriggerExitReq> _onTriggerExitEvent;
        
        private static EcsPool<OnCollisionEnterReq> _onCollisionEnterEvent;
        private static EcsPool<OnCollisionStayReq> _onCollisionStayEvent;
        private static EcsPool<OnCollisionExitReq> _onCollisionExitEvent;
        
        internal static void BindToEcsWorld(EcsWorld world)
        {
            _onTriggerEnterEvent = world.GetPool<OnTriggerEnterReq>();
            _onTriggerStayEvent = world.GetPool<OnTriggerStayReq>();
            _onTriggerExitEvent = world.GetPool<OnTriggerExitReq>();
            
            _onCollisionEnterEvent = world.GetPool<OnCollisionEnterReq>();
            _onCollisionStayEvent = world.GetPool<OnCollisionStayReq>();
            _onCollisionExitEvent = world.GetPool<OnCollisionExitReq>();
            
            _ecsWorld = world;
        }
        
        internal static void Unbind()
        {
            _onTriggerEnterEvent = null;
            _onTriggerStayEvent = null;
            _onTriggerExitEvent = null;
            
            _onCollisionEnterEvent = null;
            _onCollisionStayEvent = null;
            _onCollisionExitEvent = null;
            
            _ecsWorld = null;
        }
        
        internal static void RegisterTriggerEnterEvent(Transform senderGameObject, Collider collider)
        {
            var eventEntity = _ecsWorld.NewEntity();
            ref var eventComponent = ref _onTriggerEnterEvent.AddComponent(eventEntity);
            eventComponent.collider = collider;
            eventComponent.senderObject = senderGameObject;
        }    
        
        internal static void RegisterTriggerStayEvent(Transform senderGameObject, Collider collider)
        {
            var eventEntity = _ecsWorld.NewEntity();
            ref var eventComponent = ref _onTriggerStayEvent.AddComponent(eventEntity);
            eventComponent.collider = collider;
            eventComponent.senderObject = senderGameObject;
        }    
        
        internal static void RegisterTriggerExitEvent(Transform senderGameObject, Collider collider)
        {
            var eventEntity = _ecsWorld.NewEntity();
            ref var eventComponent = ref _onTriggerExitEvent.AddComponent(eventEntity);
            eventComponent.collider = collider;
            eventComponent.senderObject = senderGameObject;
        }    
        
        internal static void RegisterCollisionEnterEvent(Transform senderGameObject, Collider collider)
        {
            var eventEntity = _ecsWorld.NewEntity();
            ref var eventComponent = ref _onCollisionEnterEvent.AddComponent(eventEntity);
            eventComponent.collider = collider;
            eventComponent.senderObject = senderGameObject;
        }    
        
        internal static void RegisterCollisionStayEvent(Transform senderGameObject, Collider collider)
        {
            var eventEntity = _ecsWorld.NewEntity();
            ref var eventComponent = ref _onCollisionStayEvent.AddComponent(eventEntity);
            eventComponent.collider = collider;
            eventComponent.senderObject = senderGameObject;
        }    
        
        internal static void RegisterCollisionExitEvent(Transform senderGameObject, Collider collider)
        {
            var eventEntity = _ecsWorld.NewEntity();
            ref var eventComponent = ref _onCollisionExitEvent.AddComponent(eventEntity);
            eventComponent.collider = collider;
            eventComponent.senderObject = senderGameObject;
        }    
    }
}