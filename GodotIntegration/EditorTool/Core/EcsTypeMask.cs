#if TOOLS
using System;
using System.Collections.Generic;

namespace Secs
{
	public sealed partial class EcsTypeMask
	{
		internal IEnumerable<Type> GetComponents()
		{
			var list = new List<Type>();
			int cachedLength = _bitArray.Length;

			for(int i = 0; i < cachedLength; i++)
			{
				if(!_bitArray[i])
					continue;

				list.Add(EcsTypeIndexUtility.GetTypeByIndex(i));
			}

			return list;
		}
	}
}
#endif
