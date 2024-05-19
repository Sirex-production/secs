#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;

namespace Secs.Debug
{
    public class EcsSingletonEntitiesObserver : MonoBehaviour
    {
        internal EcsWorld _world;

        private void Start()
        {
            var cmpTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(e => e.GetTypes())
                .Where(e => e.GetInterfaces()
                    .Contains(typeof(IEcsSingletonComponent)));

            foreach (var type in cmpTypes)
            {
                var singletonGameObject = new GameObject($"Singleton: {type.Name}");
                var singletonEntityObserver = singletonGameObject.AddComponent<EcsSingletonEntityObserver>();
                singletonEntityObserver._singletonType = type;
                singletonEntityObserver._world = _world;
                
                singletonGameObject.transform.SetParent(transform);
            }
        
        }
    }
}
#endif