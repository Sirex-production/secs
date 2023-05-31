using System.Linq;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsFilter
	{
		public bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _entities.Count <= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetFirstEntity()
		{
			return _entities.First();
		}
	}
}