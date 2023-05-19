using System.Collections.Generic;

namespace Secs
{
    public sealed partial class EcsSystems
    {
        internal List<IEcsInitSystem> InitSystems => _initSystems;
        internal List<IEcsRunSystem> RunSystems => _runSystems;
        internal List<IEcsDisposeSystem> DisposeSystems => _disposeSystems;
    }
}