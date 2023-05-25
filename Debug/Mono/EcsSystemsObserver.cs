using System.Collections.Generic;
using UnityEngine;

namespace Secs.Debug
{
    [ExecuteInEditMode]
    public sealed class EcsSystemsObserver : MonoBehaviour
    {
        internal List<EcsSystems> ecsSystems;
        internal EcsWorld world;
    }
    
}