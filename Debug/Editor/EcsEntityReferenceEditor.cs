#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    [CustomEditor(typeof(EcsEntityReference))]
    public sealed class EcsEntityReferenceEditor : Editor
    {
        private EcsEntityReference _ecsEntityReference;
        private bool _isLinkedToEntity;
        
        private EcsEntityObserver[] _ecsEntityReferences;
        private Dictionary<int, EcsEntityObserver> _entityToEcsReference;

        private void OnEnable()
        {
            _ecsEntityReference = this.target as EcsEntityReference;
            _isLinkedToEntity = false;

            UpdateTargets();
        }

        public override void OnInspectorGUI()
        {
            if(_ecsEntityReference == null)
                return;

            int entity = _ecsEntityReference.EntityId;
            _isLinkedToEntity = entity != -1;
            
            GUI.color = _isLinkedToEntity? Color.green : Color.red ;
            
            var message = entity switch
            {
                -1 => "This entity reference does NOT have any connection to any entity",
                _ => $"This entity reference has a connection to entity no. {entity}"
            };
            
            EditorGUILayout.LabelField(message);

            CreateReferenceLinkButton();
        }

        private void UpdateTargets()
        {
            _ecsEntityReferences = FindObjectsOfType<EcsEntityObserver>(true);
            _entityToEcsReference = _ecsEntityReferences.ToDictionary(e => e.entityId, e=> e);
        }
        

        private void CreateReferenceLinkButton()
        {
            if(!_isLinkedToEntity)
                return;
            
            GUI.color = Color.yellow;
            
            if(!GUILayout.Button("Go To Observer"))
                return;

            if (!_entityToEcsReference.ContainsKey(_ecsEntityReference.EntityId))
                UpdateTargets();

            if (!_entityToEcsReference.ContainsKey(_ecsEntityReference.EntityId))
                return;

            Selection.activeObject = _entityToEcsReference[_ecsEntityReference.EntityId];

        }
    }
}
#endif