using System;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal sealed class RectDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(Rect);
        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            EditorGUI.indentLevel = currentIndentLevel + 1;
            return EditorGUILayout.RectField(objectName, value as Rect? ?? default);
        }
    }
}