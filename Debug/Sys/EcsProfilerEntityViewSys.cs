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
            _ecsWorld.OnEntityDeleted += DeleteObserver;
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
            var entityGameObject = new GameObject($"Entity {entity}");
            entityGameObject.transform.parent = _worldGameObject.transform;
            
            var entityObserver =  entityGameObject.AddComponent<EcsEntityObserver>();
            entityObserver.entityId = entity;
            entityObserver.world = _ecsWorld;
            
            _entityObservers.Add(entity, entityObserver);
        }
        
        private void DeleteObserver(int entity)
        {
            var objectToDestroy = _entityObservers[entity];
            
            _entityObservers.Remove(entity);
            Object.Destroy(objectToDestroy.gameObject);
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