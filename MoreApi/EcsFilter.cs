using System.Linq;

namespace Secs
{
	public sealed partial class EcsFilter
	{
		public bool IsEmpty => _entities.Count <= 0;

		public int GetFirstEntity()
		{
			return _entities.First();
		}
	}
}