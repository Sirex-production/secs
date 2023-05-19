using System;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal sealed class Vector3Drawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(Vector3);
        object IDrawer.Draw(Type type, string objectName, object value) => EditorGUILayout.Vector3Field(objectName, value as Vector3? ?? default);
        
    }
}