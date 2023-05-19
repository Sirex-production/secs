using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal sealed class ListDrawer : IDrawer
    {
        private object _addItem;
        bool IDrawer.IsProperType(Type type) => type.GetInterfaces().Contains((typeof(IList)));
        object IDrawer.Draw(Type type, string objectName, object value)
        {
            var list = value as IList;
            var elementType = type.GetGenericArguments()[0];

            if (list == null)
            {
                using (new EditorGUILayout.VerticalScope("box")){
                    if (GUILayout.Button("Create"))
                        return Activator.CreateInstance(type);
                    
                    EditorGUILayout.LabelField(objectName, "Null");
                    return value;
                }
            }
            
            using (new EditorGUILayout.VerticalScope("box"))
            {
                if (_addItem == null || _addItem.GetType() != elementType)
                    _addItem = Activator.CreateInstance(elementType);

                using (new EditorGUILayout.HorizontalScope())
                {
                    _addItem = EcsComponentDrawer.Draw(elementType, "new Item", _addItem);

                    if (GUILayout.Button("+"))
                    {
                        list.Add(_addItem);
                        return list;
                    }
                }

                EditorGUILayout.LabelField(objectName);
                
                if (list.Count == 0)
                {
                    EditorGUILayout.LabelField("Empty");
                    return value;
                }
                
                var cashedSize = list.Count;
                for (int i = 0; i < cashedSize; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var elem = list[i];
                        var itemName = $"Element {i}:";
                        
                        using var change = new EditorGUI.ChangeCheckScope();
                        var newVal = EcsComponentDrawer.Draw(elementType, itemName, elem);
                        
                        if (change.changed)
                            list[i] = newVal;
                        
                        if (GUILayout.Button("-"))
                        {
                            list.RemoveAt(i);
                            return list;
                        }            
                    }
                }
            }
            return list;
        }
    }
}