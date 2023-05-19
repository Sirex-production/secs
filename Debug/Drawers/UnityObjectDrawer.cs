using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class UnityObjectDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(UnityEngine.Object) || type.IsSubclassOf(typeof(UnityEngine.Object));
        object IDrawer.Draw(Type type, string objectName, object value) => EditorGUILayout.ObjectField(objectName,value as UnityEngine.Object, type,true);
        
    }
}