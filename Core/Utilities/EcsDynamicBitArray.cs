using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed class EcsDynamicBitArray
	{
		private BitArray _bitArrayMask;

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _bitArrayMask.Length;
		}

		public bool this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if(_bitArrayMask.Length <= index)
					ResizeBitArray(index);

				return _bitArrayMask[index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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