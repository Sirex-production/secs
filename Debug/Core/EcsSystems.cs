#if UNITY_EDITOR
using System.Collections.Generic;
using Secs.Debug;
using UnityEngine;
#endif
namespace Secs
{
    public sealed partial class EcsSystems
    {
#if UNITY_EDITOR
        internal List<IEcsSystem> AllSystems => _allSystems;
#endif
        public void AttachProfiler()
        {
#if UNITY_EDITOR
            if (EcsWorldsObserver.Instance == null)
                new GameObject("Profiler").AddComponent<EcsWorldsObserver>();
            
            EcsWorldsObserver.Instance?.AttachObserver(_world,this);
#endif    
        }

        public void ReleaseProfiler()
        {
#if UNITY_EDITOR            
            EcsWorldsObserver.Instance?.ReleaseObserver(_world,this);
#endif            
        }
    }
}
