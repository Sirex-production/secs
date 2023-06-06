#if UNITY_EDITOR
using System;

namespace Secs.Debug
{
    internal interface IDrawer
    {
        internal bool IsProperType(Type type);
        internal object Draw(Type type, string objectName, object value, in int currentIndentLevel);
    }
}
#endif