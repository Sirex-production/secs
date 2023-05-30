using UnityEngine;

namespace Secs
{
    public static class Extensions
    {
        public static void LinkEcsEntity(this Component component, EcsWorld world, int entityId)
        {
            if (!component.TryGetComponent(out EcsEntityReference reference))
                reference = component.gameObject.AddComponent<EcsEntityReference>();
            
            reference.Link(world, entityId);
        }
        
        public static void LinkEcsEntity(this GameObject linkedObject, EcsWorld world, int entityId)
        {
            if (!linkedObject.TryGetComponent(out EcsEntityReference reference))
                reference = linkedObject.AddComponent<EcsEntityReference>();
            
            reference.Link(world, entityId);
        }

        public static void UnlinkEcsEntity(this Component component)
        {
            if(!component.TryGetComponent(out EcsEntityReference reference))
            {
                UnityEngine.Debug.LogError($"Game object with name {component.gameObject.name} must have {nameof(EcsEntityReference)} before unlinking it");
                return;
            }

            reference.Unlink();
        }

        public static void UnlinkEcsEntity(this GameObject linkedObject)
        {
            if(!linkedObject.TryGetComponent(out EcsEntityReference reference))
            {
                UnityEngine.Debug.LogError($"Game object with name {linkedObject.name} must have {nameof(EcsEntityReference)} before unlinking it");
                return;
            }

            reference.Unlink();
        }
    }
}