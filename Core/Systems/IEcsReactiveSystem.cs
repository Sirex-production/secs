namespace Secs
{
    public interface IEcsReactiveSystem : IEcsSystem
    {
        public void Activate();

        public void Deactivate();

        public void Clear();

        public bool Filter();

        public void OnExecute();

        enum ComponentReactiveState
        {
            ComponentAdded,
            ComponentRemoved,
            Both
        }
    }
}