#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    [CustomEditor(typeof(EcsSingletonEntityObserver))]
    public sealed class EcsSingletonEntityObserverInspector : Editor
    {
        private const float REFRESH_RATE = 0.2f;
        
        private EcsSingletonEntityObserver _singletonEntityObserver;
        private float _timer;
        
        private PropertyInfo _isPresentProperty;
        private PropertyInfo _componentProperty;
        private PropertyInfo _ownerProperty;
        private MethodInfo _ownerGenericMethod;

        private bool _isPresent;
        private int _owner;
        private Color _color;
        private void OnEnable()
        {
            _singletonEntityObserver = this.target as EcsSingletonEntityObserver;
            
            if(_singletonEntityObserver == null)
                return;
            
            var world = _singletonEntityObserver._world;
            var type = _singletonEntityObserver._singletonType;
            var singletonObject = world.GetSingletonPoolAsObject(type);
            
            _isPresentProperty = singletonObject.GetType().GetProperty("IsPresent");
            _componentProperty = world.GetSingletonPoolAsObject(type).GetType().GetProperty("Component");
            _ownerProperty = world.GetSingletonPoolAsObject(type).GetType().GetProperty("OwnerEntity");

            RefreshCashedSingletonComponents();
            _color = EntityColorPallet.GetRandomColor();
            EditorApplication.update += OnUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        public override void OnInspectorGUI()
        {
            var world = _singletonEntityObserver._world;

            if (!_isPresent)
            {
                EditorGUILayout.LabelField($"That singleton Component does not exist");
                return;
            }
            GUI.backgroundColor = _color;
            using (new EditorGUILayout.VerticalScope("box"))
            {
                var componentType = _componentProperty.PropertyType;
                
                EditorGUILayout.LabelField($"{componentType.Name} owned by {_owner}");
                    
                _ownerGenericMethod ??= world.GetType()
                    .GetMethod(nameof(EcsWorld.GetSingletonItem), BindingFlags.NonPublic | BindingFlags.Instance)?
                    .MakeGenericMethod(componentType);
                
                if(_ownerGenericMethod == null)
                    return;
                
                var componentResult = _ownerGenericMethod.Invoke(world, null);
                var fields = componentResult.GetType().GetFields();
                    
                foreach (var fieldInfo in fields) 
                {
                    var fieldValue = fieldInfo.GetValue(componentResult);
                    EcsComponentDrawer.Draw(fieldInfo.FieldType, fieldInfo.Name, fieldValue, 3);
                }
                    
                EditorGUILayout.LabelField("");
            }
        }
        
        private void OnUpdate()
        {
            if (_timer > 0)
            {
                _timer -= Time.unscaledDeltaTime;
                return;
            }

            _timer = REFRESH_RATE;
       
            RefreshCashedSingletonComponents();
        }

        private void RefreshCashedSingletonComponents()
        {
            var type = _singletonEntityObserver._singletonType;
            var world = _singletonEntityObserver._world;
            var isPresentObject = _isPresentProperty.GetValue(world.GetSingletonPoolAsObject(type));
            
            _isPresent = isPresentObject != null && (bool)isPresentObject;
            
            if(!_isPresent)
                return;
            
            var ownerObject = _ownerProperty.GetValue(world.GetSingletonPoolAsObject(type));

            if (ownerObject is int ownerEntity)
            {
                _owner = ownerEntity;
            }
          
        }
    }
}
#endif