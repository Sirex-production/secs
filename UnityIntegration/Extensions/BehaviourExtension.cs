using System;
using Secs;
using Secs.UnityIntegration;
using UnityEngine;

namespace Secs.UnityIntegration 
{
    public static class BehaviourExtension
    {
        public static void Link(this GameObject linkedObject, EcsWorld world, int entityId)
        {
            if (!linkedObject.TryGetComponent(out EcsEntityReference reference))
                reference = linkedObject.AddComponent<EcsEntityReference>();
            
            reference.Link( world, entityId);
        }
        
        public static void UnLink(this GameObject linkedObject, EcsWorld world, int entityId)
        {
            if (!linkedObject.TryGetComponent(out EcsEntityReference reference))
                throw new ArgumentException("The object must have Entity Reference before unlinking it");
            
            reference.Unlink( world, entityId);
        }
    }
}