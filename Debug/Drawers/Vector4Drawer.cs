using System;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal sealed class Vector4Drawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(Vector4);
        object IDrawer.Draw(Type type, string objectName, object value) => EditorGUILayout.Vector4Field(objectName, value as Vector4? ?? default);
    }
}