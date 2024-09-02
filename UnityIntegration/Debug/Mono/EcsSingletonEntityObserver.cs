#if UNITY_EDITOR

using System;
using UnityEngine;

namespace Secs.Debug
{
    public sealed class EcsSingletonEntityObserver : MonoBehaviour
    {
        internal Type _singletonType;
        internal EcsWorld _world;
    }
}
#endif