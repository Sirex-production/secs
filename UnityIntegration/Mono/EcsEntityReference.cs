using System;
using UnityEngine;

namespace Secs.UnityIntegration
{
    public sealed class EcsEntityReference : MonoBehaviour
    {
        private int _entityId = -1;
        private EcsWorld _world;

        public int EntityId => _entityId;
        public EcsWorld World => _world;

        public void Link(EcsWorld ecsWorld, int entityId)
        {
            if (this._entityId != -1)
                throw new ArgumentException("This entity reference is occupied by other entity");

            _world = ecsWorld;
            _entityId = entityId;
        }
        
        public void Unlink(EcsWorld ecsWorld, int entity)
        {
            if (this._entityId == -1)
                throw new ArgumentException("Link entity before unlinking it");
            
            _world = null;
            _entityId = -1;
        }
    }
}