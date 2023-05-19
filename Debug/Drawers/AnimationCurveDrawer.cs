using System;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal sealed class AnimationCurveDrawer : IDrawer
    {
        bool IDrawer.IsProperType(Type type) => type == typeof(AnimationCurve);
        object IDrawer.Draw(Type type, string objectName, object value) => EditorGUILayout.CurveField(objectName,  value as AnimationCurve);
    }
}