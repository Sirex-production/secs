#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class StringDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(string);
        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            EditorGUI.indentLevel = currentIndentLevel + 1;
            return EditorGUILayout.TextField(objectName, value as string);
        }
    }
}
#endif