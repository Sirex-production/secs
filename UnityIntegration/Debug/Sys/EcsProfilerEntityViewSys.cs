#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secs.Debug 
{
    public sealed class EcsProfilerEntityViewSys : IEcsDisposeSystem
    {
        private EcsWorld _ecsWorld;
        private List<EcsSystems> _systems;
        private GameObject _worldGameObject;
        private EcsSystemsObserver _ecsSystemsObserver;
        private EcsSingletonEntitiesObserver _singletonEntitiesObserver;
        private readonly Dictionary<int, EcsEntityObserver> _entityObservers = new();
        
        public EcsProfilerEntityViewSys(EcsWorld world, List<EcsSystems> ecsSys)
        {
            _ecsWorld = world;
            _systems = ecsSys;
            
            CreateWorld();
            CreateSingletonObserver();

            _ecsWorld.OnEntityCreated += CreateNewObserver;
            _ecsWorld.OnEntityDeleted += DeactivateObserver;
            
            _ecsWorld.OnComponentAddedToEntity += OnComponentAddedToEntity;
            _ecsWorld.OnComponentDeletedFromEntity += OnComponentDeletedFromEntity;
            
            
            foreach (var ecsWorldAliveEntity in _ecsWorld.AliveEntities)
                CreateNewObserver(ecsWorldAliveEntity);
        }

        private void CreateSingletonObserver()
        {
            var singletonObserverGo = new GameObject($"Singleton Components");
            
            _singletonEntitiesObserver = singletonObserverGo.AddComponent<EcsSingletonEntitiesObserver>();
            _singletonEntitiesObserver._world = _ecsWorld;
            
            singletonObserverGo.transform.SetParent(EcsWorldsObserver.Instance.gameObject.transform);
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
        
        
        private void OnComponentDeletedFromEntity(int entityId, Type _)
        {
            RebuildGOName(entityId);
        }

        private void OnComponentAddedToEntity(int entityId, Type _)
        {
            RebuildGOName(entityId);
        }

        private void RebuildGOName(int entityId)
        {
            var sb = new StringBuilder();
            var types = _ecsWorld.GetEntityComponentsTypeMask(entityId).GetComponents();

            var enumerable = types as Type[] ?? types.ToArray();
            if (!enumerable.Any())
            {
                _entityObservers[entityId].gameObject.name = $"Entity {entityId}";
                return;
            }
            
            sb.Append($"Entity {entityId}: ");

            foreach (var type in enumerable)
            {
                sb.Append($"{type.Name} ");
            }
                
            _entityObservers[entityId].gameObject.name = sb.ToString();
        }
        
        
        public void OnDispose()
        {
            if (_ecsWorld != null)
            {
                _ecsWorld.OnEntityCreated -= CreateNewObserver;
                _ecsWorld.OnEntityDeleted -= DeactivateObserver;
            
                _ecsWorld.OnComponentAddedToEntity -= OnComponentAddedToEntity;
                _ecsWorld.OnComponentDeletedFromEntity -= OnComponentDeletedFromEntity;
            }

            Object.Destroy(_worldGameObject);
            _ecsWorld = null;
            _worldGameObject = null;
            _entityObservers.Clear();
        }
    }
}
#endif