using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class FloatDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(float);
        object IDrawer.Draw(Type type, string objectName, object value) => EditorGUILayout.FloatField(objectName, (float?)value ?? default);
        
    }
}