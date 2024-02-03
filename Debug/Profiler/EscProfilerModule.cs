using Unity.Profiling.Editor;
using Unity.Profiling;

namespace Secs.Profiler
{
#if SECS_PROFILING

    [ProfilerModuleMetadata("SECS")]
    public sealed class EscProfilerModule : ProfilerModule
    {
        private static readonly ProfilerCounterDescriptor[] _profilerDescriptors = {
            new ("GC Allocated In Frame", ProfilerCategory.Memory),
            new ("Scripts", ProfilerCategory.Scripts),
            new ("Others", ProfilerCategory.Scripts),
            new ("GarbageCollector", ProfilerCategory.Memory),
        };
        
        public EscProfilerModule() : base(_profilerDescriptors) { }

        public override ProfilerModuleViewController CreateDetailsViewController()
        {
            return new EcsModuleViewController(ProfilerWindow);
        }
        
    }
        
#endif
}