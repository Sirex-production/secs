using System;
using System.Runtime.CompilerServices;

namespace Secs
{
	public partial class EcsWorld
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEcsPoolNonGeneric GetNonGenericPool(Type cmpType)
		{
			return (IEcsPoolNonGeneric)GetPool(cmpType);
		}
	}
}