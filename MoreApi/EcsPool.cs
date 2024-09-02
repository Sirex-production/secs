using System.Runtime.CompilerServices;

namespace Secs
{
    public partial class EcsPool<T>
    {
        /// <summary>
        /// Gets component from entity if it is present
        /// </summary>
        /// <param name="component">Reference to component</param>
        /// <returns>TRUE if component was found. FALSE otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetComponent(int entity, out T component)
        {
            if(!_world.HasCmp<T>(entity))
            {
                component = default;
                return false;
            }

            component = GetComponent(entity);
            return true;
        }

        /// <summary>
        /// Deletes component from entity if it is present
        /// </summary>
        /// <returns>TRUE if component was deleted. FALSE otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDelComponent(in int entity)
        {
            if(!_world.HasCmp<T>(entity))
                return false;

            DelComponent(entity);
            return true;
        }

        /// <summary>
        /// Deletes components from all entities in the filter
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DelAllComponents(EcsFilter filter)
        {
            foreach (var entity in filter)
            {
                if(HasComponent(entity))
                    DelComponent(entity);
            }
        }

        /// <summary>
        /// Gets component from entity if it is present or adds it if it doesn't exist
        /// </summary>
        /// <param name="entity">Entity to get component from</param>
        /// <returns>Reference to component</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetOrAddComponent(in int entity)
        {
            if(HasComponent(entity))
                return ref GetComponent(entity);
			
            return ref AddComponent(entity);
        }
    }
}