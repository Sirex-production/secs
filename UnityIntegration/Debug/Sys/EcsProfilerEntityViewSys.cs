#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secs.Debug 
{
    public sealed class EcsProfilerEntityViewSys : IEcsDisposeSystem
    {
        private EcsWorld _ecsWorld;
        private GameObject _worldGameObject;
        private EcsSystemsObserver _ecsSystemsObserver;
        private EcsSingletonEntitiesObserver _singletonEntitiesObserver;
        
        private readonly List<EcsSystems> _systems;
        private readonly Dictionary<int, EcsEntityObserver> _entityObservers = new();
        private readonly Dictionary<int, string> _entityNameCache = new();

        private readonly float _refreshInterval = .3f;
        private float _timeSinceLastUpdate = .0f;

        public EcsProfilerEntityViewSys(EcsWorld world, List<EcsSystems> ecsSys)
        {
            _ecsWorld = world;
            _systems = ecsSys;
            
            CreateWorld();
            CreateSingletonObserver();
            
            _ecsWorld.OnEntityCreated += CreateNewObserver;
            _ecsWorld.OnEntityDeleted += DeactivateObserver;

            foreach (var ecsWorldAliveEntity in _ecsWorld.AliveEntities)
                CreateNewObserver(ecsWorldAliveEntity);
            
            EditorApplication.update += OnUpdate;
        }

        private void OnUpdate()
        {
            _timeSinceLastUpdate += Time.deltaTime;

            if (_timeSinceLastUpdate < _refreshInterval)
                return;

            _timeSinceLastUpdate = 0.0f;
            

            foreach (var (entityId, _) in _entityObservers)
            {
                RebuildGOName(entityId);
            }
        }

        private void CreateSingletonObserver()
        {
            var singletonObserverGo = new GameObject("Singleton Components");
            _singletonEntitiesObserver = singletonObserverGo.AddComponent<EcsSingletonEntitiesObserver>();
            _singletonEntitiesObserver._world = _ecsWorld;
            singletonObserverGo.transform.SetParent(EcsWorldsObserver.Instance.gameObject.transform);
        }
        
        private void CreateWorld()
        {
            _worldGameObject = new GameObject("World");
            _ecsSystemsObserver = _worldGameObject.AddComponent<EcsSystemsObserver>();
            _ecsSystemsObserver.world = _ecsWorld;
            _ecsSystemsObserver.ecsSystems = _systems;
            _worldGameObject.transform.SetParent(EcsWorldsObserver.Instance.gameObject.transform);
        }
        
        private void CreateNewObserver(int entity)
        {
            if (_entityObservers.ContainsKey(entity))
                return;
            
            var entityGameObject = new GameObject($"Entity {entity}");
            entityGameObject.transform.parent = _worldGameObject.transform;
            var entityObserver = entityGameObject.AddComponent<EcsEntityObserver>();
            entityObserver.entityId = entity;
            entityObserver.world = _ecsWorld;
            _entityObservers.Add(entity, entityObserver);

            RebuildGOName(entity);
        }
        
        private void DeactivateObserver(int entity)
        {
            if (_entityObservers.ContainsKey(entity))
            {
                RebuildGOName(entity);
            }
        }

        private void RebuildGOName(int entityId)
        {
            var sb = new StringBuilder();
            var types = _ecsWorld.GetEntityComponentsTypeMask(entityId).GetComponents();
            var enumerable = types as Type[] ?? types.ToArray();
            
            if(!_entityObservers.TryGetValue(entityId, out var observer))
                return;

            if (!enumerable.Any())
            {
                SetEntityName(entityId, $"Entity {entityId}");
                observer.gameObject.SetActive(false);
                return;
            }

            sb.Append($"Entity {entityId}: ");
            observer.gameObject.SetActive(true);
            foreach (var type in enumerable)
            {
                sb.Append($"{type.Name} ");
            }

            SetEntityName(entityId, sb.ToString());
        }

        private void SetEntityName(int entityId, string newName)
        {
            if (_entityObservers.TryGetValue(entityId, out var observer))
            {
                if (_entityNameCache.TryGetValue(entityId, out var cachedName) && cachedName == newName)
                {
                    return;
                }

                observer.gameObject.name = newName;
                _entityNameCache[entityId] = newName;
            }
        }

        public void OnDispose()
        {
            if (_ecsWorld != null)
            {
                _ecsWorld.OnEntityCreated -= CreateNewObserver;
                _ecsWorld.OnEntityDeleted -= DeactivateObserver;
            }

            Object.Destroy(_worldGameObject);
            _ecsWorld = null;
            _worldGameObject = null;
            _entityObservers.Clear();
            _entityNameCache.Clear();
            EditorApplication.update -= OnUpdate;
        }

  
    }
}
#endif
