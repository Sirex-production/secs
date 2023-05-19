using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Secs.Debug
{
    internal static class EcsComponentDrawer  
    {
        private static readonly Dictionary<Type, IDrawer> _simpleTypeDrawers = new Dictionary<Type, IDrawer>()
        {
            {typeof(AnimationCurveDrawer), new AnimationCurveDrawer()},
            {typeof(bool), new BoolDrawer()},
            {typeof(Color), new ColorDrawer()},
            {typeof(float), new FloatDrawer()},
            {typeof(int), new IntDrawer()},
            {typeof(Rect), new RectDrawer()},
            {typeof(string), new StringDrawer()},
            {typeof(Vector2), new Vector2Drawer()},
            {typeof(Vector3), new Vector3Drawer()},
            {typeof(Vector4), new Vector4Drawer()},
        };
        
        private static readonly List<IDrawer> _conditionTypeDrawers = new List<IDrawer>()
        {
            new ArrayDrawer(),
            new DictionaryDrawer(),
            new EnumDrawer(),
            new ListDrawer(),
            new UnityObjectDrawer(),
            new ObjectDrawer()
            
        };
        
        internal static object Draw(Type type, string objectName, object value)
        {
            if (_simpleTypeDrawers.ContainsKey(type))
            {
                return _simpleTypeDrawers[type].Draw(type, objectName, value);
            }

            foreach (var drawer in _conditionTypeDrawers)
            {
                if (drawer.IsProperType(type))
                    return drawer.Draw(type, objectName, value);
            }

            EditorGUILayout.LabelField($"{value}");
            return value;
        }
    }
}