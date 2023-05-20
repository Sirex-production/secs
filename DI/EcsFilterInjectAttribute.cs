using System;

namespace Secs
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class EcsFilterInjectAttribute : Attribute { }
	
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class EcsIncludeAttribute : Attribute
	{
		public readonly Type[] includeTypes;
		
		public EcsIncludeAttribute(params Type[] includeTypes)
		{
			this.includeTypes = includeTypes;
		}
	}
	
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class EcsExcludeAttribute : Attribute
	{
		public readonly Type[] excludeTypes;
		
		public EcsExcludeAttribute(params Type[] excludeTypes)
		{
			this.excludeTypes = excludeTypes;
		}
	}
}