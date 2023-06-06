using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Secs
{
	public static class EcsTypeIndexUtility
	{
		private static Dictionary<Type, int> _typeToIndex = new(64);
		private static Dictionary<int, Type> _indexToType = new(64);
		
		private static int _lastUsedIndex = -1;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetIndexOfType(Type type)
		{
			if(_typeToIndex.ContainsKey(type))
				return _typeToIndex[type];

			_lastUsedIndex++;
			_typeToIndex.Add(type, _lastUsedIndex);
			_indexToType.Add(_lastUsedIndex, type);
			
			return _lastUsedIndex;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetIndexOfType<T>()
		{
			var type = typeof(T);
			
			if(_typeToIndex.ContainsKey(type))
				return _typeToIndex[type];

			_lastUsedIndex++;
			_typeToIndex.Add(type, _lastUsedIndex);
			_indexToType.Add(_lastUsedIndex, type);
			
			return _lastUsedIndex;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type GetTypeByIndex(int index)
		{
			if(!_indexToType.ContainsKey(index))
				throw new EcsException(nameof(EcsTypeIndexUtility), "There is such type for such index");

			return _indexToType[index];
		}
	}
}