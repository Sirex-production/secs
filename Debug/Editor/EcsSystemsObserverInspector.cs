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
        private void OnEnable()
        {
            _ecsSystemsObserver = this.target as EcsSystemsObserver;
        }

        private void OnDisable()
        {
            _ecsSystemsObserver = null;
        }

        public override void OnInspectorGUI()
        {
             if(_ecsSystemsObserver == null)
                 return;

             if (GUILayout.Button("New Entity"))
                 _ecsSystemsObserver.world.NewEntity();
             
             foreach (var sys in _ecsSystemsObserver.ecsUnitSystemsObservers)
             {
                 using (new EditorGUILayout.VerticalScope("box"))
                 {
                     EditorGUILayout.LabelField("IEcsInit systems:");
                     using (new EditorGUILayout.VerticalScope("box")){
                         foreach (var initSys in sys.initSystems)
                         {
                             EditorGUILayout.LabelField(initSys.GetType().Name);
                         }
                     }
                     
                     EditorGUILayout.LabelField("IEcsRun systems:");
                     using (new EditorGUILayout.VerticalScope("box")){
                         foreach (var runSys in sys.runSystems)
                         {
                             EditorGUILayout.LabelField(runSys.GetType().Name);
                         }
                     }
                     
                     EditorGUILayout.LabelField("IEcsDispose systems:");
                     using (new EditorGUILayout.VerticalScope("box")){
                         foreach (var disposeSys in sys.disposeSystems)
                         {
                             EditorGUILayout.LabelField(disposeSys.GetType().Name);
                         }
                     }
                 }
             }
        }
    }
}