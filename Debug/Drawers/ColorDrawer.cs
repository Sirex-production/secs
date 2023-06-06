#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal class ColorDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(Color);
        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            EditorGUI.indentLevel = currentIndentLevel + 1;
            return EditorGUILayout.ColorField(objectName, value as Color? ?? default);
        }
    }
}
#endif