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
        
        public static void UnlinkEcsEntity(this GameObject linkedObject)
        {
            if(!linkedObject.TryGetComponent(out EcsEntityReference reference))
            {
                Debug.LogError("The object must have Entity Reference before unlinking it");
                return;
            }

            reference.Unlink();
        }
    }
}