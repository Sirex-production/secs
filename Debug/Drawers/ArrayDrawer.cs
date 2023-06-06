using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal sealed class ArrayDrawer : IDrawer
    {
        private object _addItem;
        bool IDrawer.IsProperType(Type type) => type.IsArray;
        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            var array = value as Array;
            var elementType = type.GetElementType();
            var newIndentLevel = currentIndentLevel + 1;

            EditorGUI.indentLevel = newIndentLevel;
            if (array == null)
            {
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    if (GUILayout.Button("Create") && elementType != null)
                        return Array.CreateInstance(elementType, new int[type.GetArrayRank()]);

                    EditorGUILayout.LabelField(objectName, "Null");
                    return value;
                }
            }

            if (elementType == null)
                return value;
            
            using (new EditorGUILayout.VerticalScope())
            {
                if (_addItem == null || _addItem.GetType() != elementType)
                    _addItem = elementType.IsSubclassOf(typeof(UnityEngine.Object)) ? null : Activator.CreateInstance(elementType);

                using (new EditorGUILayout.HorizontalScope()){
                    _addItem = EcsComponentDrawer.Draw(elementType, "new Item", _addItem,newIndentLevel);
                    
                    if (GUILayout.Button("+"))
                        return AddArrayValue(array, elementType, _addItem);
                }
                
                EditorGUILayout.LabelField(objectName);
                
                if (array.Length == 0)
                {
                    EditorGUILayout.LabelField("Empty");
                    return value;
                }

                var cashedSize = array.Length;
                for (int i = 0; i < cashedSize; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var elem = array.GetValue(i);
                        var itemName = $"Element {i}:";
                        
                        using var fieldChange = new EditorGUI.ChangeCheckScope();
                        var newVal = EcsComponentDrawer.Draw(elementType, itemName, elem,newIndentLevel);

                        if (fieldChange.changed)
                            array.SetValue(newVal, i);

                        if (GUILayout.Button("-"))
                            return RemoveArrayValueAt(array, elementType, i);
                    }
                }
            }

            return array;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Array RemoveArrayValueAt(Array array, Type elementType, int removeAt)
        {
            var arrayList = new ArrayList(array);
            arrayList.RemoveAt(removeAt);
            return arrayList.ToArray(elementType);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Array AddArrayValue(Array array, Type elementType, object value)
        {
            var arrayList = new ArrayList(array);
            arrayList.Insert(arrayList.Count, value);
            return arrayList.ToArray(elementType);
        }
    }
}