using UnityEngine;

namespace Secs
{
    public sealed class EcsEntityReference : MonoBehaviour
    {
        private int _entityId = -1;
        private EcsWorld _world;

        public int EntityId => _entityId;
        public EcsWorld World => _world;

        public void Link(EcsWorld ecsWorld, int entityId)
        {
            if(_entityId != -1)
            {
                UnityEngine.Debug.LogError($"Trying to override entity id on already occupied {nameof(EcsEntityReference)} on game object {gameObject.name}");
                return;
            }

            _world = ecsWorld;
            _entityId = entityId;
        }
        
        public void Unlink()
        {
            if(_entityId == -1)
            {
                UnityEngine.Debug.LogError($"Link entity before unlinking it on game object {gameObject.name}");
                return;
            }

            _world = null;
            _entityId = -1;
        }
    }
}