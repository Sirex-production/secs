#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class InterfaceDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type.IsInterface;
        
        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            using (new EditorGUILayout.HorizontalScope("box"))
            {
                EditorGUILayout.LabelField($"{type.Name} (Interface)");
                
                if (value == null)
                    EditorGUILayout.LabelField($"Null");
                else
                    return  EcsComponentDrawer.Draw(value.GetType(), objectName, value,currentIndentLevel);
                
                return null;
            }
        }
    }
}
#endif