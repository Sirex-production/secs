using Secs;

namespace Secs
{
    public sealed class DisposePhysicsRequestsSys : IEcsRunSystem
    {
        private readonly EcsWorld _ecsWorld;
        
        private readonly EcsFilter _onTriggerEnterFilter;
        private readonly EcsFilter _onTriggerStayEventFilter;
        private readonly EcsFilter _onTriggerExitEventFilter;
        
        private readonly EcsFilter _onCollisionEnterEventFilter;
        private readonly EcsFilter _onCollisionStayEventFilter;
        private readonly EcsFilter _onCollisionExitEventFilter;

        public DisposePhysicsRequestsSys(EcsWorld ecsWorld)
        {
            _ecsWorld = ecsWorld;

            _onTriggerEnterFilter = _ecsWorld.GetFilter(EcsMatcher.Include(typeof(OnTriggerEnterReq)).End());
            _onTriggerStayEventFilter = _ecsWorld.GetFilter(EcsMatcher.Include(typeof(OnTriggerStayReq)).End());
            _onTriggerExitEventFilter = _ecsWorld.GetFilter(EcsMatcher.Include(typeof(OnTriggerExitReq)).End());
            
            _onCollisionEnterEventFilter = _ecsWorld.GetFilter(EcsMatcher.Include(typeof(OnCollisionEnterReq)).End());
            _onCollisionStayEventFilter = _ecsWorld.GetFilter(EcsMatcher.Include(typeof(OnCollisionStayReq)).End());
            _onCollisionExitEventFilter = _ecsWorld.GetFilter(EcsMatcher.Include(typeof(OnCollisionExitReq)).End());
        }
        
        public void OnRun()
        {
            foreach (var entity in _onTriggerEnterFilter)
                _ecsWorld.DelEntity(entity);
            
            foreach (var entity in _onTriggerStayEventFilter)
                _ecsWorld.DelEntity(entity);
            
            foreach (var entity in _onTriggerExitEventFilter)
                _ecsWorld.DelEntity(entity);

            foreach (var entity in _onCollisionEnterEventFilter)
                _ecsWorld.DelEntity(entity);
            
            foreach (var entity in _onCollisionStayEventFilter)
                _ecsWorld.DelEntity(entity);
            
            foreach (var entity in _onCollisionExitEventFilter)
                _ecsWorld.DelEntity(entity);
        }
    }
}