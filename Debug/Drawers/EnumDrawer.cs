using System;
using UnityEditor;

namespace Secs.Debug
{
    internal sealed class EnumDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type.IsEnum;
        object IDrawer.Draw(Type type, string objectName, object value)
        {
           return type.IsDefined(typeof(FlagsAttribute),false) ? 
               EditorGUILayout.EnumFlagsField(objectName, (Enum)value) :
               EditorGUILayout.EnumPopup(objectName, (Enum)value);
        }
    }
}