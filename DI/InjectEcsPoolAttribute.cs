using System;

namespace Secs
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class InjectEcsPoolAttribute : Attribute
	{
		public readonly Type componentType;

		public InjectEcsPoolAttribute(Type componentType)
		{
			if(!componentType.IsValueType)
				throw new ArgumentException("Pool can not be created with non struct component");
			
			this.componentType = componentType;
		}
	}
}