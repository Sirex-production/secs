namespace Secs
{
    public interface IEcsDestroyable : IEcsComponent
    {
        public void OnDestroy();
    }
}