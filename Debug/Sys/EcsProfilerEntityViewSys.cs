using System.Collections.Generic;
using UnityEngine;

namespace Secs.Debug 
{
    public sealed class EcsProfilerEntityViewSys : IEcsDisposeSystem
    {
        private EcsWorld _ecsWorld;
        private List<EcsSystems> _systems;
        private GameObject _worldGameObject;
        private readonly Dictionary<int, EcsEntityObserver> _entityObservers = new Dictionary<int, EcsEntityObserver>();

        public EcsProfilerEntityViewSys(EcsWorld world,EcsSystems ecsSys)
        {
            _ecsWorld = world;
            _systems = new List<EcsSystems>() { ecsSys };
            
            CreateWorld();
            
            _ecsWorld.OnEntityCreated += CreateNewObserver;
            _ecsWorld.OnEntityDeleted += DeleteObserver;
        }
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
            var worldObserver = _worldGameObject.AddComponent<EcsSystemsObserver>();

            worldObserver.world = _ecsWorld;
            worldObserver.ecsUnitSystemsObservers ??= new List<EcsUnitSystemsObserver>();
            
            foreach (var sys in _systems)
            {
                worldObserver.ecsUnitSystemsObservers.Add(new EcsUnitSystemsObserver()
                {
                    initSystems = sys.InitSystems,
                    runSystems =  sys.RunSystems,
                    disposeSystems = sys.DisposeSystems
                });
            }
            
            Object.DontDestroyOnLoad(this._worldGameObject);
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