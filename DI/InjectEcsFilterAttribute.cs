using System;

namespace Secs
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class InjectEcsFilterAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class Inc : Attribute
	{
		public readonly Type[] includeTypes;
		
		public Inc(params Type[] includeTypes)
		{
			this.includeTypes = includeTypes;
		}
	}
	
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class Exc : Attribute
	{
		public readonly Type[] excludeTypes;
		
		public Exc(params Type[] excludeTypes)
		{
			this.excludeTypes = excludeTypes;
		}
	}
}