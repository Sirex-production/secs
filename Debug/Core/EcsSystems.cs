using System.Collections.Generic;
using Secs.Debug;
using UnityEngine;

namespace Secs
{
    public sealed partial class EcsSystems
    {
        internal List<IEcsSystem> AllSystems => _allSystems;

        public void AttachProfiler()
        {
            if (EcsWorldsObserver.Instance == null)
                new GameObject("Profiler").AddComponent<EcsWorldsObserver>();
            
            EcsWorldsObserver.Instance.AttachObserver(_world,this);
        }

        public void ReleaseProfiler()
        {
            EcsWorldsObserver.Instance.ReleaseObserver(_world,this);
        }
    }
}