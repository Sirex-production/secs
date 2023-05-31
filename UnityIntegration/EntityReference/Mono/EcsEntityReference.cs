using UnityEngine;

namespace Secs
{
    /// <summary>
    /// MonoBehaviour that holds reference to ECS entity and world
    /// </summary>
    /// <remarks>Use this class to retrieve information from game objects about entity that is attached to it</remarks>
    public sealed class EcsEntityReference : MonoBehaviour
    {
        private int _entityId = -1;
        private EcsWorld _world;

        /// <summary>
        /// Attached entity id
        /// </summary>
        public int EntityId => _entityId;
        /// <summary>
        /// Attached world to which entity belongs
        /// </summary>
        public EcsWorld World => _world;
        
        /// <summary>
        /// Links given Ecs world and entity to the EcsEntityReference
        /// </summary>
        /// <param name="ecsWorld">World where entity exists</param>
        /// <param name="entityId">Entity that will be attached</param>
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
        
        /// <summary>
        /// Removed link to entity
        /// </summary>
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