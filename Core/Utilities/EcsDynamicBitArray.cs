using System;
using System.Collections;

namespace Secs
{
	public class EcsDynamicBitArray
	{
		private BitArray _bitArrayMask;

		public int Length => _bitArrayMask.Length;
		
		public bool this[int index]
		{
			get
			{
				if(_bitArrayMask.Length <= index)
					ResizeBitArray(index);

				return _bitArrayMask[index];
			}

			set
			{
				if(_bitArrayMask.Length <= index)
					ResizeBitArray(index);

				_bitArrayMask[index] = value;
			}
		}
		
		public EcsDynamicBitArray()
		{
			_bitArrayMask = new BitArray(256, false);
		}
		
		public EcsDynamicBitArray(int amountOfAllocatedBits)
		{
			_bitArrayMask = new BitArray(amountOfAllocatedBits, false);
		}

		private void ResizeBitArray(int minimalLength)
		{
			int currentLength = _bitArrayMask.Length;
			
			if(minimalLength < currentLength)
				return;

			int targetLength = Math.Max(minimalLength, currentLength * 2);
			var createdBitArray = new BitArray(targetLength);

			for(int i = 0; i < _bitArrayMask.Length; i++) 
				createdBitArray[i] = _bitArrayMask[i];

			_bitArrayMask = createdBitArray;
		}
	}
}