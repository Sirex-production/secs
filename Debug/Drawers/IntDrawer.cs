﻿using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class IntDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(int);
        object IDrawer.Draw(Type type, string objectName, object value,in int currentIndentLevel)
        {
            EditorGUI.indentLevel = currentIndentLevel + 1;
            return EditorGUILayout.IntField(objectName, (int?)value ?? default);
        }
    }
}