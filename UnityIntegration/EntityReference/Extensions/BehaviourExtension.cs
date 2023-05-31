using UnityEngine;

namespace Secs
{
    public static class Extensions
    {
        /// <summary>
        /// Links entity to specific EcsEntityReference and world
        /// </summary>
        /// <param name="world">World to which entity belongs</param>
        /// <param name="entityId">Entity that will be attached</param>
        public static void LinkEcsEntity(this Component component, EcsWorld world, int entityId)
        {
            if (!component.TryGetComponent(out EcsEntityReference reference))
                reference = component.gameObject.AddComponent<EcsEntityReference>();
            
            reference.Link(world, entityId);
        }
        
        /// <summary>
        /// Links entity to specific EcsEntityReference and world
        /// </summary>
        /// <param name="world">World to which entity belongs</param>
        /// <param name="entityId">Entity that will be attached</param>
        public static void LinkEcsEntity(this GameObject linkedObject, EcsWorld world, int entityId)
        {
            if (!linkedObject.TryGetComponent(out EcsEntityReference reference))
                reference = linkedObject.AddComponent<EcsEntityReference>();
            
            reference.Link(world, entityId);
        }

        /// <summary>
        /// Unlinks entity from specific EcsEntityReference
        /// </summary>
        public static void UnlinkEcsEntity(this Component component)
        {
            if(!component.TryGetComponent(out EcsEntityReference reference))
            {
                UnityEngine.Debug.LogError($"Game object with name {component.gameObject.name} must have {nameof(EcsEntityReference)} before unlinking it");
                return;
            }

            reference.Unlink();
        }

        /// <summary>
        /// Unlinks entity from specific EcsEntityReference
        /// </summary>
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