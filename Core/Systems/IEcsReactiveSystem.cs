using System;

namespace Secs
{
    public interface IEcsReactiveSystem : IEcsSystem
    {
        public void Activate();
        public void Deactivate();
    }
}