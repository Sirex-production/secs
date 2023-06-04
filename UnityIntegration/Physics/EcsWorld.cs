namespace Secs
{
    public sealed partial class EcsWorld
    {
        public void BindPhysics()
        {
            EcsPhysics.BindToEcsWorld(this);
        }
        
        public void UnbindPhysics()
        {
            EcsPhysics.Unbind();
        }
    }
}