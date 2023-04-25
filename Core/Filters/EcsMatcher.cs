using System;

namespace Secs
{
	public sealed class EcsMatcher
	{
		private readonly EcsTypeMask _includeTypeMask;
		private readonly EcsTypeMask _excludeTypeMask;

		private EcsMatcher(Type[] includeTypes)
		{
			_includeTypeMask = new EcsTypeMask(includeTypes);
		}
		
		private EcsMatcher(Type[] includeTypes, Type[] excludeTypes)
		{
			_includeTypeMask = new EcsTypeMask(includeTypes);
			_excludeTypeMask = new EcsTypeMask(excludeTypes);

			if(_includeTypeMask.HasCommonTypesWith(_excludeTypeMask))
				throw new ArgumentException("Include types overlaps with exclude types");
		}

		internal bool IsIncluded(Type type)
		{
			return _includeTypeMask.ContainsType(type);
		}
		
		internal bool IsExcluded(Type type)
		{
			if(_excludeTypeMask == null)
				return false;
			
			return _excludeTypeMask.ContainsType(type);
		}

		internal bool IsSameAsIncludeMask(EcsTypeMask otherMask)
		{
			return _includeTypeMask == otherMask;
		}
		
		internal bool IsSameAsExcludeMask(EcsTypeMask otherMask)
		{
			return _excludeTypeMask == otherMask;
		}

#region Comparing
		public override bool Equals(object obj)
		{
			if(obj is not EcsMatcher ecsFilterMask)
				return false;

			return Equals(ecsFilterMask);
		}

		private bool Equals(EcsMatcher other)
		{
			if(other is null)
				return false;
			
			return _includeTypeMask == other._includeTypeMask && _excludeTypeMask == other._excludeTypeMask;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_includeTypeMask, _excludeTypeMask);
		}
		
		public static bool operator==(EcsMatcher first, EcsMatcher second)
		{
			if(first is null && second is null)
				return true;
			
			if(first is null || second is null)
				return false;
			
			return first.Equals(second);
		}

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

			public EcsMatcherBuilder Exclude(params Type[] excludeComponentTypes)
			{
				if(_excludeComponentTypes != null)
					throw new ArgumentException("Exclude types were already assigned");
				
				_excludeComponentTypes = excludeComponentTypes;
				return this;
			}

			public EcsMatcher End()
			{
				return new EcsMatcher(_includeComponentTypes, _excludeComponentTypes);
			}
		}
#endregion
	}
}