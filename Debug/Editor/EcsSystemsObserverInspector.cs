using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    [CustomEditor(typeof(EcsSystemsObserver))]
    public sealed class EcsSystemsObserverInspector: Editor
    {
        private EcsSystemsObserver _ecsSystemsObserver = null;
        private readonly Dictionary<EcsSystems, Dictionary<Type, List<string>>> _cashedSystems = new ();

        private void OnEnable()
        {
            _ecsSystemsObserver = this.target as EcsSystemsObserver;
            
            if(_ecsSystemsObserver == null)
                return;
            
            SortSystems();
        }

        private void OnDisable()
        {
            _ecsSystemsObserver = null;
            _cashedSystems.Clear();
        }

        public override void OnInspectorGUI()
        {
             if(_ecsSystemsObserver == null)
                 return;

             if (GUILayout.Button("New Entity"))
                 _ecsSystemsObserver.world.NewEntity();

             var index = 0;
             foreach (var cashedSystems in _cashedSystems)
             {
                 EditorGUI.indentLevel = 0;
                 EditorGUILayout.LabelField($"Systems : {index++}");
                 
                 using (new EditorGUILayout.VerticalScope())
                 {
                     foreach (var unitSys in cashedSystems.Value)
                     {
                         EditorGUI.indentLevel = 1;
                         
                         using (new EditorGUILayout.VerticalScope("box"))
                         {
                             EditorGUILayout.LabelField(unitSys.Key.Name);

                             foreach (var sys in unitSys.Value)
                             {
                                 EditorGUI.indentLevel = 4;
                                 EditorGUILayout.LabelField(sys);
                             }
                         }
                     }
                 }
             }
        }

        private void SortSystems()
        {
            foreach (var unitSystemsObserver in _ecsSystemsObserver.ecsSystems)
            {
                _cashedSystems.Add(unitSystemsObserver, new Dictionary<Type, List<string>>()
                {
                    {typeof(IEcsInitSystem),new List<string>()},
                    {typeof(IEcsRunSystem),new List<string>()},
                    {typeof(IEcsDisposeSystem),new List<string>()}
                });
 
                foreach (var sys in unitSystemsObserver.AllSystems)
                {
                    if (sys is IEcsInitSystem)
                        _cashedSystems[unitSystemsObserver][typeof(IEcsInitSystem)].Add(sys.GetType().Name);
                    
                    if (sys is IEcsRunSystem)
                        _cashedSystems[unitSystemsObserver][typeof(IEcsRunSystem)].Add(sys.GetType().Name);

                    if (sys is IEcsDisposeSystem)
                        _cashedSystems[unitSystemsObserver][typeof(IEcsDisposeSystem)].Add(sys.GetType().Name);
                } 
            }
        }
    }
}