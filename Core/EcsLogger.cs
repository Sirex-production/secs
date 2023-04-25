using UnityEngine;

namespace Secs
{
	public static class EcsLogger
	{
		public static void HeapAlloc<T>()
		{
			Debug.Log($"[Logger] Heap allocation of type [{typeof(T)}]");
		}
		
		public static void HeapAlloc<T>(int size)
		{
			Debug.Log($"[Logger] Heap allocation of type [{typeof(T)}] size [{size}]");
		}
	}
}