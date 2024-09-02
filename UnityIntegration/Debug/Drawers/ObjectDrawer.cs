#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal sealed class ObjectDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type.IsClass || type.IsValueType;

        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            var newIndentLevel = currentIndentLevel + 1;
            
            EditorGUI.indentLevel = newIndentLevel;
            
            if ( !type.IsSerializable || type.GetCustomAttribute<EcsDontDrawFields>(true) != null)
            {
                using (new EditorGUILayout.HorizontalScope("box"))
                {
                    EditorGUILayout.LabelField(type.Name);
                    EditorGUILayout.LabelField(value==null?"null":value.GetType().ToString());
                }

                return value;
            }
            
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(type.Name);

                if (value == null)
                    return GUILayout.Button("Create") ? Activator.CreateInstance(type) : null;
                
                var cashedFields = type.GetFields();
                foreach (var f in cashedFields)
                {
                    var fieldValue = f.GetValue(value);
                    using var change = new EditorGUI.ChangeCheckScope();
                    var newVal = EcsComponentDrawer.Draw(f.FieldType, f.Name, fieldValue,newIndentLevel);
                    
                    if (change.changed)
                        f.SetValue(value,newVal);
                    
                }
            }
           
            return value;
        }
    }
}
#endif