using System;

namespace Secs
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class EcsWorldInject : Attribute
	{
		
	}
}