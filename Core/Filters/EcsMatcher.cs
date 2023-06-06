using System;
using System.Runtime.CompilerServices;

namespace Secs
{
	public sealed class EcsMatcher
	{
		internal readonly EcsTypeMask includeTypeMask;
		internal readonly EcsTypeMask excludeTypeMask;

		private EcsMatcher(Type[] includeTypes)
		{
			includeTypeMask = new EcsTypeMask(includeTypes);
		}
		
		private EcsMatcher(Type[] includeTypes, Type[] excludeTypes)
		{
			includeTypeMask = new EcsTypeMask(includeTypes);
			excludeTypeMask = new EcsTypeMask(excludeTypes);

			if(includeTypeMask.HasCommonTypesWith(excludeTypeMask))
				throw new EcsException(this, "Include types overlaps with exclude types");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsIncluded(Type type)
		{
			return includeTypeMask.ContainsType(type);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsExcluded(Type type)
		{
			if(excludeTypeMask == null)
				return false;
			
			return excludeTypeMask.ContainsType(type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsSameAsIncludeMask(EcsTypeMask otherMask)
		{
			return includeTypeMask == otherMask;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsSameAsExcludeMask(EcsTypeMask otherMask)
		{
			return excludeTypeMask == otherMask;
		}

#region Comparing
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj)
		{
			if(obj is not EcsMatcher ecsFilterMask)
				return false;

			return Equals(ecsFilterMask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool Equals(EcsMatcher other)
		{
			if(other is null)
				return false;
			
			return includeTypeMask == other.includeTypeMask && excludeTypeMask == other.excludeTypeMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return HashCode.Combine(includeTypeMask, excludeTypeMask);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator==(EcsMatcher first, EcsMatcher second)
		{
			if(first is null && second is null)
				return true;
			
			if(first is null || second is null)
				return false;
			
			return first.Equals(second);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator!=(EcsMatcher first, EcsMatcher second)
		{
			if(first is null && second is null)
				return true;
			
			if(first is null || second is null)
				return false;
			
			return !first.Equals(second);
		}
#endregion

#region Builder
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static EcsMatcherBuilder Include(params Type[] includeComponentTypes)
		{
			return new EcsMatcherBuilder(includeComponentTypes);
		}
		
		public struct EcsMatcherBuilder
		{
			private readonly Type[] _includeComponentTypes;
			private Type[] _excludeComponentTypes;

			public EcsMatcherBuilder(Type[] includeComponentTypes)
			{
				_includeComponentTypes = includeComponentTypes;
				_excludeComponentTypes = null;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public EcsMatcherBuilder Exclude(params Type[] excludeComponentTypes)
			{
				if(_excludeComponentTypes != null)
					throw new EcsException(this, "Exclude types were already assigned");
				
				_excludeComponentTypes = excludeComponentTypes;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public EcsMatcher End()
			{
				return new EcsMatcher(_includeComponentTypes, _excludeComponentTypes);
			}
		}
#endregion
	}
}