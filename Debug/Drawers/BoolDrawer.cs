#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class BoolDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(bool);
        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            return EditorGUILayout.Toggle(objectName, (bool)value);
        }
    }
}
#endif