#if UNITY_EDITOR
using UnityEngine;

namespace Secs.Debug
{
    internal static class EntityColorPallet
    {
        private static Color[] _colors = new[]
        {
            new Color(0.4f, 0.6f, 0.8f),
            new Color(0.8f, 0.6f, 0.8f),
            new Color(0.1f, 0.6f, 0.1f),
            new Color(0.5f, 0.3f, 0.5f),
            new Color(0.73f, 0.2f, 0.23f),
            new Color(0.73f, 0.9f, 0.3f),
            new Color(0.53f, 0.9f, 0.7f),
            new Color(0.53f, 0.6f, 0.2f),
            new Color(0.9f, 0.6f, 0.2f),
            new Color(0.74f, 0.2f, 0.6f),
        };

        internal static Color GetColorByIndex(int index)
        {
            return _colors[(index % _colors.Length)];
        }
    }
}

#endif