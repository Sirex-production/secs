#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace Secs.Debug 
{
    public sealed class EcsProfilerEntityViewSys : IEcsDisposeSystem
    {
        private EcsWorld _ecsWorld;
        private List<EcsSystems> _systems;
        private GameObject _worldGameObject;
        private EcsSystemsObserver _ecsSystemsObserver;
        private readonly Dictionary<int, EcsEntityObserver> _entityObservers = new();
        
        public EcsProfilerEntityViewSys(EcsWorld world, List<EcsSystems> ecsSys)
        {
            _ecsWorld = world;
            _systems = ecsSys;
            
            CreateWorld();
            
            _ecsWorld.OnEntityCreated += CreateNewObserver;
            _ecsWorld.OnEntityDeleted += DeactivateObserver;
            
            foreach (var ecsWorldAliveEntity in _ecsWorld.AliveEntities)
                CreateNewObserver(ecsWorldAliveEntity);
        }
        
        private void CreateWorld()
        {
            _worldGameObject = new GameObject($"World");
            _ecsSystemsObserver = _worldGameObject.AddComponent<EcsSystemsObserver>();

            _ecsSystemsObserver.world = _ecsWorld;
            _ecsSystemsObserver.ecsSystems = _systems;
            
            _worldGameObject.transform.SetParent(EcsWorldsObserver.Instance.gameObject.transform);
        }
        
        private void CreateNewObserver(int entity)
        {
            if (_entityObservers.ContainsKey(entity))
            {
                _entityObservers[entity].gameObject.SetActive(true);
                return;
            }
            
            var entityGameObject = new GameObject($"Entity {entity}");
            entityGameObject.transform.parent = _worldGameObject.transform;
            
            var entityObserver =  entityGameObject.AddComponent<EcsEntityObserver>();
            entityObserver.entityId = entity;
            entityObserver.world = _ecsWorld;
            
            _entityObservers.Add(entity, entityObserver);
        }
        
        private void DeactivateObserver(int entity)
        {
            if (!_entityObservers.ContainsKey(entity))
                return;
            
            _entityObservers[entity].gameObject.SetActive(false);
        }
        
        public void OnDispose()
        {
            Object.Destroy(_worldGameObject);
            _ecsWorld = null;
            _worldGameObject = null;
            _entityObservers.Clear();
        }
    }
}
#endif