 
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Secs.Debug;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug 
{
    [CustomEditor(typeof(EcsEntityObserver))]
    public sealed class EcsEntityObserverInspector : Editor
    {
        private const float REFRESH_RATE = 0.2f;
        private static int _maximumNumberOfComponents = 32;
        
        private Type[] _cashedComponentTypes = new Type[_maximumNumberOfComponents];
        private object[] _cashedComponents = new object[_maximumNumberOfComponents];
        private int _numberOfComponents = 0;
        
        private EcsEntityObserver _entityObserver = null;
        private int _entityId = -1;
        private EcsWorld _ecsWorld = null;

        private readonly Dictionary<string, Type> _popupStringToTypeDiction = new();
        private IEnumerable<Type> _cmpTypes; 
        private int _popupIndex = 0;
        private string[] _popupOptions;
        private object _popupObject;

        private float _refresh_timer = 0f;
        private void Awake()
        {
            var list = new List<string>();
            
            _cmpTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(e => e.GetTypes())
                .Where(e => e.GetInterfaces()
                    .Contains(typeof(IEcsComponent)));
            
            foreach (var cmpType in _cmpTypes)
            {
                list.Add(cmpType.Name);
                _popupStringToTypeDiction.Add(cmpType.Name, cmpType);
            }

            _popupOptions = list.ToArray();
        }

        void OnEnable()
        {
            _entityObserver = target as EcsEntityObserver;
            
            if (_entityObserver == null) 
                return;
            
            _entityId = _entityObserver.entityId;
            _ecsWorld= _entityObserver.world;

            if (_entityId == -1 || _ecsWorld == null)
                return;
            
            InitComponents();
            
            var cmpType = _popupStringToTypeDiction[_popupOptions[_popupIndex]];
            _popupObject = Activator.CreateInstance(cmpType);

            _refresh_timer = 0f;
            EditorApplication.update += OnUpdate;
        }

        private void OnDisable()
        {
            _entityObserver = null;
            _entityId = -1;
            _ecsWorld = null;
            _popupObject = null;
            
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            _refresh_timer += Time.deltaTime;
            
            if(_refresh_timer < REFRESH_RATE)
                return;
            
            _refresh_timer = 0f;

            var shouldRepaint = false;
            for (int i = 0; i < _numberOfComponents; i++)
            {
                ref var componentValue = ref _cashedComponents[i];

                var result = _ecsWorld
                    .GetType()
                    .GetMethod(nameof(EcsWorld.IsSame),BindingFlags.NonPublic | BindingFlags.Instance)?
                    .MakeGenericMethod(_cashedComponentTypes[i])
                    .Invoke(_ecsWorld, new object[] { _entityId, componentValue});
                
                if (result != null && (shouldRepaint =! (bool)result))
                {
                    var typeValue = _ecsWorld
                        .GetType()
                        .GetMethod(nameof(EcsWorld.GetItem),BindingFlags.NonPublic | BindingFlags.Instance)?
                        .MakeGenericMethod(_cashedComponentTypes[i])
                        .Invoke(_ecsWorld, new object[] { _entityId,});

                    _cashedComponents[i] = typeValue;
                }
            }
            
            if(shouldRepaint)
                Repaint();
        }
        public override void OnInspectorGUI()
        {
            if (_entityObserver == null) 
                return;
            
            serializedObject.Update();
            

            if (_entityId == -1 || _ecsWorld == null)
            {
                EditorGUILayout.LabelField("The observer does not observe any entity");
                return;
            }

            CreateAddComponentScope();

            for (int i = 0; i < _numberOfComponents; i++)
            {
                var type = _cashedComponentTypes[i];
                
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    if(CreateRemoveComponentScope(type))
                        return;
                    
                    CreateViewComponentScope(type, i);
                }
            }
        }
 
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitComponents()
        {
            _numberOfComponents = 0;
            var types = _ecsWorld.GetEntityComponentsTypeMask(_entityId).GetComponents();
            
            foreach (var type in types)
            {
                var typeValue = _ecsWorld
                    .GetType()
                    .GetMethod(nameof(EcsWorld.GetItem),BindingFlags.NonPublic | BindingFlags.Instance)?
                    .MakeGenericMethod(type)
                    .Invoke(_ecsWorld, new object[] { _entityId,});
                
                if (typeValue == null) 
                    continue;
                
                if (_numberOfComponents >= _maximumNumberOfComponents)
                {
                    _maximumNumberOfComponents += _maximumNumberOfComponents / 2;
                        
                    var cashedComponents = _cashedComponents;
                    var cashedComponentTypes = _cashedComponentTypes;

                    _cashedComponents = new object[_maximumNumberOfComponents];
                    _cashedComponentTypes = new Type[_maximumNumberOfComponents];
                        
                    cashedComponents.CopyTo(_cashedComponents,0);
                    cashedComponentTypes.CopyTo(_cashedComponentTypes,0);
                }
                
                _cashedComponents[_numberOfComponents] = typeValue;
                _cashedComponentTypes[_numberOfComponents] = type;
                _numberOfComponents++;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateAddComponentScope()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using var popupChange = new EditorGUI.ChangeCheckScope();
                _popupIndex = EditorGUILayout.Popup("Components:", _popupIndex, _popupOptions);
                var cmpType = _popupStringToTypeDiction[_popupOptions[_popupIndex]];
                
                if (popupChange.changed)
                    _popupObject = Activator.CreateInstance(cmpType);
                
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    var cashedFields = cmpType.GetFields();
                    foreach (var field in cashedFields)
                    {
                        using var fieldChange = new EditorGUI.ChangeCheckScope();
                        var newValue = EcsComponentDrawer.Draw(field.FieldType, field.Name, field.GetValue(_popupObject));
                        if (!fieldChange.changed)
                            continue;
                        
                        field.SetValue(_popupObject, newValue);
                    }

                    if (GUILayout.Button("Add"))
                    {
                        _ecsWorld
                            .GetType()
                            .GetMethod(nameof(EcsWorld.AddItem), BindingFlags.NonPublic | BindingFlags.Instance)?
                            .MakeGenericMethod(cmpType)
                            .Invoke(_ecsWorld, new object[]
                                {
                                    _entityId, 
                                    _popupObject
                                }
                            );
                        
                        InitComponents();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CreateRemoveComponentScope(in Type type)
        {
            using (new EditorGUILayout.HorizontalScope("box"))
            {
                EditorGUILayout.LabelField($"{type.Name}");
                if (GUILayout.Button("-"))
                {
                    _ecsWorld
                        .GetType()
                        .GetMethod(nameof(EcsWorld.DeleteComponent),BindingFlags.NonPublic | BindingFlags.Instance)?
                        .MakeGenericMethod(type)
                        .Invoke(_ecsWorld, new object[]
                            {
                                _entityId
                            }
                        );
                            
                    InitComponents();
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateViewComponentScope(in Type type, in int componentId)
        {
            var cashedFields = type.GetFields();
            foreach (var field in cashedFields)
            {
                using (new EditorGUILayout.HorizontalScope("box"))
                {
                    using (var change = new EditorGUI.ChangeCheckScope())
                    {
                        var newValue = EcsComponentDrawer.Draw(field.FieldType, field.Name, field.GetValue(_cashedComponents[componentId]));
                        if (!change.changed)
                            continue;

                        field.SetValue(_cashedComponents[componentId], newValue);
                        _ecsWorld
                            .GetType()
                            .GetMethod(nameof(EcsWorld.ReplaceItem), BindingFlags.NonPublic | BindingFlags.Instance)?
                            .MakeGenericMethod(type)
                            .Invoke(_ecsWorld, new object[]
                                {
                                    _entityId, 
                                    _cashedComponents[componentId]
                                }
                            );
                    }
                }
            }
        }
    }
}