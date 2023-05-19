using System;
using UnityEngine;

namespace Secs.Debug
{
    [ExecuteInEditMode]
    public sealed class EcsEntityObserver : MonoBehaviour
    {
        internal int entityId = -1;
        internal EcsWorld world = null;
    }
    
}