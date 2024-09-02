using System;
using System.Collections.Generic;
using System.Linq;

namespace Secs
{
    public sealed partial class EcsTypeMask
    {
        internal IEnumerable<Type> GetComponents()
        {
            var list = new List<Type>();
            int cashedLength = _bitArray.Length;
            
            for (int i = 0; i < cashedLength; i++)
            {
                if (!_bitArray[i]) 
                    continue;
                
                var type = EcsTypeIndexUtility.GetTypeByIndex(i);
                list.Add(type);
            }
            
            return list;
        }
    }
}