using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace Secs.Profiler
{
#if SECS_PROFILING

    public class EcsProfiler
    {
        private readonly Dictionary<string,ProfilerMarker> _profilerMarkers = new();

        public Action CreateProfilerWrapperForRunActionSystem(IEcsRunSystem runSystem)
        {
            return () =>
            {
                if (!_profilerMarkers.TryGetValue(runSystem.ToString(), out var profilerMarker))
                {
                    profilerMarker = new ProfilerMarker(runSystem.ToString());
                  
                    _profilerMarkers.Add(runSystem.ToString(), profilerMarker);
                }
                
                profilerMarker.Begin();
                runSystem.OnRun();
                profilerMarker.End();
            };
        }
        
    }

#endif
}