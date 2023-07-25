using System;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed partial class EcsTypeMask : IEquatable<EcsTypeMask>
	{
		private readonly EcsDynamicBitArray _bitArray = new();

		public EcsTypeMask() { }
		
		public EcsTypeMask(Type[] types)
		{
			if(types == null || types == Type.EmptyTypes)
				return;

			if(types.Length < 1)
				throw new EcsException(this, "Cannot create mask out of empty array of types");

			foreach(var type in types) 
				AddType(type);
		}

		/// <summary>
		/// Checks if mask contains type <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T">Type to check</typeparam>
		/// <returns>TRUE if mask contains type <typeparamref name="T"/>, FALSE otherwise</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool ContainsType<T>() where T : struct
		{
			return ContainsType(typeof(T));
		}
		
		/// <summary>
		/// Checks if mask contains type <paramref name="type"/>
		/// </summary>
		/// <param name="type">Type to check</param>
		/// <returns>TRUE if mask contains type <paramref name="type"/>, FALSE otherwise</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool ContainsType(Type type)
		{
			int indexOfType = EcsTypeIndexUtility.GetIndexOfType(type);

			return _bitArray[indexOfType];
		}
		
		/// <summary>
		/// Checks if mask includes all types from <paramref name="ecsTypeMask"/>
		/// </summary>
		/// <param name="ecsTypeMask">Mask to check</param>
		/// <returns>TURE if mask includes all types from <paramref name="ecsTypeMask"/>, FALSE otherwise</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool Includes(EcsTypeMask ecsTypeMask)
		{
			var otherBitArray = ecsTypeMask._bitArray;

			for(int i = 0; i < _bitArray.Length; i++)
			{
				if(!_bitArray[i] && otherBitArray[i])
					return false;
			}

			return true;
		}

		/// <summary>
		/// Checks if mask includes any types from <paramref name="ecsTypeMask"/>
		/// </summary>
		/// <param name="otherTypeMask">Mask to check</param>
		/// <returns>TURE if mask includes any types from <paramref name="otherTypeMask"/>, FALSE otherwise</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		/// <summary>
		/// Adds type <typeparamref name="T"/> to mask
		/// </summary>
		/// <typeparam name="T">Type to add</typeparam>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void AddType<T>() where T : struct
		{
			AddType(typeof(T));
		}
		
		/// <summary>
		/// Adds type <paramref name="type"/> to mask
		/// </summary>
		/// <param name="type">Type to add</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void AddType(Type type)
		{
			int indexOfType = EcsTypeIndexUtility.GetIndexOfType(type);
			_bitArray[indexOfType] = true;
		}

		/// <summary>
		/// Removes type <typeparamref name="T"/> from mask
		/// </summary>
		/// <typeparam name="T">Type to remove</typeparam>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void RemoveType<T>() where T : struct
		{
			int indexOfType = EcsTypeIndexUtility.GetIndexOfType(typeof(T));
			_bitArray[indexOfType] = false;
		}
		
		/// <summary>
		/// Checks if mask contains any types
		/// </summary>
		/// <returns>TURE if mask contains any types, FALSE otherwise</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool HasAnyTypes()
		{
			return _bitArray.PositiveBitsCount > 0;
		}
		
#region Comparing
		public override bool Equals(object obj)
		{
			if(obj is not EcsTypeMask other)
				return false;
			
			return Equals(other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(EcsTypeMask other)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(EcsTypeMask first, EcsTypeMask second)
		{
			if(first is null && second is null)
				return true;
			
			if(first is null || second is null)
				return false;
			
			return first.Equals(second);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(EcsTypeMask first, EcsTypeMask second)
		{
			if(first is null && second is null)
				return true;
			
			if(first is null || second is null)
				return false;
			
			return !first.Equals(second);
		}
#endregion
	}
}