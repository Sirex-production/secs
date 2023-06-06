#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal sealed class QuaternionDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(Quaternion);

        object IDrawer.Draw(Type type, string objectName, object value, in int currentIndentLevel)
        {
            EditorGUI.indentLevel = currentIndentLevel + 1;

            var trueValue = (Quaternion)value;
            var quaternionAsVector4 = new Vector4(trueValue.x, trueValue.y, trueValue.z, trueValue.w);
            var vectorValue =  EditorGUILayout.Vector4Field(objectName, quaternionAsVector4);

            return new Quaternion(vectorValue.x,vectorValue.y,vectorValue.z,vectorValue.w);
        }
    }
}
#endif