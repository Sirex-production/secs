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
        public bool TryGet(int entity, out T component)
        {
            if(!_world.Has<T>(entity))
            {
                component = default;
                return false;
            }

            component = Get(entity);
            return true;
        }

        /// <summary>
        /// Deletes component from entity if it is present
        /// </summary>
        /// <returns>TRUE if component was deleted. FALSE otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDel(in int entity)
        {
            if(!_world.Has<T>(entity))
                return false;

            Del(entity);
            return true;
        }

        /// <summary>
        /// Deletes components from all entities in the filter
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DelAll(EcsFilter filter)
        {
            foreach (var entity in filter)
            {
                if(Has(entity))
                    Del(entity);
            }
        }

        /// <summary>
        /// Gets component from entity if it is present or adds it if it doesn't exist
        /// </summary>
        /// <param name="entity">Entity to get component from</param>
        /// <returns>Reference to component</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetOrAdd(in int entity)
        {
            if(Has(entity))
                return ref Get(entity);
			
            return ref Add(entity);
        }
    }
}