using System;
using System.Collections.Generic;
using UnityEngine;

namespace Secs.Debug
{
    [ExecuteInEditMode]
    public sealed class EcsWorldsObserver : MonoBehaviour
    {
        private static EcsWorldsObserver _ecsWorldsObserver;
        private EcsWorld _debugWorld;
        private EcsSystems _systems;
        
        private Dictionary<EcsWorld, List<EcsSystems>> _observedWorlds = new();
        private Dictionary<EcsWorld,EcsProfilerEntityViewSys> _storedProfilerSystems = new();
        
        public static EcsWorldsObserver Instance => _ecsWorldsObserver;
        private void Awake()
        {
            if (_ecsWorldsObserver == null)
            {
                DontDestroyOnLoad(gameObject);
                
                _ecsWorldsObserver = this;
                _debugWorld = new EcsWorld();
                _systems = new EcsSystems(_debugWorld);
                
               return;
            }
            
            Destroy(this);
        }

        private void OnDestroy()
        {
            if (this != _ecsWorldsObserver) 
                return;
            
            _systems.FireDisposeSystems();
            _systems = null;
            _ecsWorldsObserver = null;
            _debugWorld = null;
        }

        internal void AttachObserver(EcsWorld world, EcsSystems systems)
        {
            if (!_observedWorlds.ContainsKey(world))
            {
                var sys = new List<EcsSystems>() { systems };
                var profiler = new EcsProfilerEntityViewSys(world, sys);
                
                _observedWorlds.Add(world, sys);
                _storedProfilerSystems.Add(world, profiler);
                
                _systems.Add(profiler);
                return;
            }
            
            _observedWorlds[world].Add(systems);
        }
        
        internal void ReleaseObserver(EcsWorld world, EcsSystems systems)
        {
            if (!_observedWorlds.ContainsKey(world) || !_observedWorlds[world].Contains(systems)) 
                return;
            
            _observedWorlds[world].Remove(systems);
            
            if(_observedWorlds[world].Count <= 0)
                _storedProfilerSystems[world].OnDispose();
        }
    }
}