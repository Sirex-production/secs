using System.Collections.Generic;
using Secs.Debug;

namespace Secs
{
    public sealed partial class EcsSystems
    {
        private EcsProfilerEntityViewSys _profiler;
        internal List<IEcsSystem> AllSystems => _allSystems;

        public void AttachProfiler()
        {
            _profiler = new EcsProfilerEntityViewSys(_world, this);
            Add(_profiler);
        }
    }
}