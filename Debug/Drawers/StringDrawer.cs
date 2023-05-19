using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class StringDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(string);
        object IDrawer.Draw(Type type, string objectName, object value) => EditorGUILayout.TextField(objectName, value as string);
        
    }
}