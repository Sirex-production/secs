using System;

namespace Secs.Debug
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class EcsDontDrawFields : Attribute
    {
        public EcsDontDrawFields() { }
    }
}