using System.Collections.Generic;
using UnityEngine;

namespace Secs.Debug
{
    [ExecuteInEditMode]
    public sealed class EcsSystemsObserver : MonoBehaviour
    {
        internal List<EcsUnitSystemsObserver> ecsUnitSystemsObservers;
        internal EcsWorld world;
    }


    internal struct EcsUnitSystemsObserver
    {
        internal List<IEcsInitSystem> initSystems;
        internal List<IEcsRunSystem> runSystems;
        internal List<IEcsDisposeSystem> disposeSystems;
    }
}