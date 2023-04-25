using System;
using System.Collections.Generic;

namespace Secs
{
	public static class EcsTypeIndexUtility
	{
		private static Dictionary<Type, int> _typeToIndex = new(64);
		private static Dictionary<int, Type> _indexToType = new(64);
		
		private static int _lastUsedIndex = -1;
		
		public static int GetIndexOfType(Type type)
		{
			if(_typeToIndex.ContainsKey(type))
				return _typeToIndex[type];

			_lastUsedIndex++;
			_typeToIndex.Add(type, _lastUsedIndex);
			_indexToType.Add(_lastUsedIndex, type);
			
			return _lastUsedIndex;
		}
		
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

		public static Type GetTypeByIndex(int index)
		{
			if(!_indexToType.ContainsKey(index))
				throw new ArgumentException("There is such type for such index");

			return _indexToType[index];
		}
	}
}