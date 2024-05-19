#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal sealed class Vector2Drawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(Vector2);
        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            EditorGUI.indentLevel = currentIndentLevel + 1;
            return EditorGUILayout.Vector2Field(objectName, value as Vector2? ?? default);
        }
    }
}
#endif