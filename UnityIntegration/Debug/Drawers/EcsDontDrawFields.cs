using System;

namespace Secs.Debug
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class EcsDontDrawFields : Attribute
    {
        public EcsDontDrawFields() { }
    }
}
 