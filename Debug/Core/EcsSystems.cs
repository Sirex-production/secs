using System.Collections.Generic;
using Secs.Debug;

namespace Secs
{
    public sealed partial class EcsSystems
    {
        private EcsProfilerEntityViewSys _profiler;
        internal List<IEcsInitSystem> InitSystems => _initSystems;
        internal List<IEcsRunSystem> RunSystems => _runSystems;
        internal List<IEcsDisposeSystem> DisposeSystems => _disposeSystems;
        
        public void AttachProfiler()
        {
            _profiler = new EcsProfilerEntityViewSys(_world, this);
            Add(_profiler);
        }
    }
}