#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class ObjectDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type.IsSerializable && (type.IsClass || type.IsValueType);

        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            var newIndentLevel = currentIndentLevel + 1;
            
            EditorGUI.indentLevel = newIndentLevel;
            
            if (type.GetCustomAttribute<EcsDontDrawFields>(true) != null)
            {
                EditorGUILayout.LabelField(type.Name);
                return value;
            }
            
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(type.Name);
                var cashedFields = type.GetFields();
                foreach (var f in cashedFields)
                {
                    var fieldValue = f.GetValue(value);
                    using var change = new EditorGUI.ChangeCheckScope();
                    var newVal = EcsComponentDrawer.Draw(f.FieldType, f.Name, fieldValue,newIndentLevel);
                    if (change.changed)
                    {
                        f.SetValue(value,newVal);
                    }
                }
            }
           
            return value;
        }
    }
}
#endif