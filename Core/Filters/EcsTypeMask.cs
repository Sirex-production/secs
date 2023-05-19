using System;

namespace Secs
{
	public sealed partial class EcsTypeMask
	{
		private EcsDynamicBitArray _bitArray;
		
		public EcsTypeMask()
		{
			_bitArray = new EcsDynamicBitArray();
		}

		public EcsTypeMask(Type[] types)
		{
			_bitArray = new EcsDynamicBitArray();
			
			if(types == null)
				return;

			if(types.Length < 1)
				throw new ArgumentException("Cannot create mask out of empty array of types");

			foreach(var type in types)
			{
				int indexOfType = EcsTypeIndexUtility.GetIndexOfType(type);
				_bitArray[indexOfType] = true;
			}
		}

		internal bool ContainsType<T>() where T : struct
		{
			int indexOfType = EcsTypeIndexUtility.GetIndexOfType<T>();

			return _bitArray[indexOfType];
		}
		
		internal bool ContainsType(Type type)
		{
			int indexOfType = EcsTypeIndexUtility.GetIndexOfType(type);

			return _bitArray[indexOfType];
		}

		internal bool HasCommonTypesWith(EcsTypeMask otherTypeMask)
		{
			int longestMaskCount = Math.Max(_bitArray.Length, otherTypeMask._bitArray.Length);

			for(int i = 0; i < longestMaskCount; i++)
			{
				if(!_bitArray[i])
					continue;
				
				if(_bitArray[i] == otherTypeMask._bitArray[i])
					return true;
			}

			return false;
		}

		internal void AddType<T>() where T : struct
		{
			int indexOfType = EcsTypeIndexUtility.GetIndexOfType(typeof(T));
			
			_bitArray[indexOfType] = true;
		}
		
		internal void RemoveType<T>() where T : struct
		{
			int indexOfType = EcsTypeIndexUtility.GetIndexOfType(typeof(T));
			
			_bitArray[indexOfType] = false;
		}

#region Comparing
		public override bool Equals(object obj)
		{
			if(obj is not EcsTypeMask mask)
				return false;

			return Equals(mask);
		}

		private bool Equals(EcsTypeMask other)
		{
			if(other is null)
				return false;
			
			var otherBitMask = other._bitArray;
			int longestMaskLength = Math.Max(_bitArray.Length, otherBitMask.Length);

			for(int i = 0; i < longestMaskLength; i++)
				if(_bitArray[i] != otherBitMask[i])
					return false;

			return true;
		}

		public override int GetHashCode()
		{
			return _bitArray != null ? _bitArray.GetHashCode() : 0;
		}

		public static bool operator ==(EcsTypeMask ecsTypeMask1, EcsTypeMask ecsTypeMask2)
		{
			return ecsTypeMask1.Equals(ecsTypeMask2);
		}
		
		public static bool operator !=(EcsTypeMask ecsTypeMask1, EcsTypeMask ecsTypeMask2)
		{
			return !ecsTypeMask1.Equals(ecsTypeMask2);
		}
#endregion
	}
}