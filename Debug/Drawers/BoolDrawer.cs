using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class BoolDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(bool);
        object IDrawer.Draw(Type type, string objectName, object value) => EditorGUILayout.Toggle(objectName, (bool)value);
    }
}