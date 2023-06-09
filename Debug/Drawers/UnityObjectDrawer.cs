﻿#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class UnityObjectDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(UnityEngine.Object) || type.IsSubclassOf(typeof(UnityEngine.Object));
        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            EditorGUI.indentLevel = currentIndentLevel + 1;
            return EditorGUILayout.ObjectField(objectName, value as UnityEngine.Object, type, true);
        }
    }
}
#endif