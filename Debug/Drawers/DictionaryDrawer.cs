using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal sealed class DictionaryDrawer : IDrawer
    {
        private object _addItemKey;
        private object _addItemValue;
        bool IDrawer.IsProperType(Type type)=> type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        object IDrawer.Draw(Type type, string objectName, object value)
        {
            var dictionary = value as IDictionary;
            var keyType = type.GetGenericArguments()[0];
            var valueType = type.GetGenericArguments()[1];
            
            if (dictionary == null)
            {
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    if (GUILayout.Button("Create"))
                        return Activator.CreateInstance(type);

                    EditorGUILayout.LabelField(objectName, "Null");
                    return value;
                }
            }

            using (new EditorGUILayout.VerticalScope("box"))
            {
                if (_addItemKey == null || _addItemKey.GetType() != keyType)
                    _addItemKey = Activator.CreateInstance(keyType);
                
                if (_addItemValue == null || _addItemValue.GetType() != valueType)
                    _addItemValue = Activator.CreateInstance(valueType);
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    _addItemKey = EcsComponentDrawer.Draw(keyType, "new Key", _addItemKey);
                    _addItemValue = EcsComponentDrawer.Draw(valueType, "new Value", _addItemValue);
              
                    if (GUILayout.Button("+"))
                    {
                        dictionary.Add(_addItemKey,_addItemValue);
                        return dictionary;
                    }
                }
                
                EditorGUILayout.LabelField(objectName);
                
                if (dictionary.Count == 0)
                {
                    EditorGUILayout.LabelField("Empty");
                    return value;
                }

                var keys = new ArrayList(dictionary.Keys);
                foreach (var key in keys)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var valueName = $"Value :";
                        using var change = new EditorGUI.ChangeCheckScope();
                        
                        EditorGUILayout.LabelField($"Key : {key}");
                        var newValue = EcsComponentDrawer.Draw(valueType, valueName, dictionary[key]);

                        if (change.changed)
                            dictionary[key] = newValue;
                        
                        if (GUILayout.Button("-"))
                        {
                            dictionary.Remove(key);
                            return dictionary;
                        }         
                    }
                    
                }
            }
            return dictionary;
        }
    }
}