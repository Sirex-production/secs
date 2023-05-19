using System;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal class ColorDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(Color);
        object IDrawer.Draw(Type type, string objectName, object value) =>
            EditorGUILayout.ColorField(objectName, value as Color? ?? default);
    }
}