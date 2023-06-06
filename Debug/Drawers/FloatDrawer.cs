#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class FloatDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(float);
        object IDrawer.Draw(Type type, string objectName, object value,in int currentIndentLevel)
        {
            EditorGUI.indentLevel = currentIndentLevel + 1;
            return EditorGUILayout.FloatField(objectName, (float?)value ?? default);
        }
    }
}
#endif