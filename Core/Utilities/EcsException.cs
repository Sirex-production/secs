using System;

namespace Secs
{
	public sealed class EcsException : Exception
	{
		public EcsException(object sender, object message) : base($"[{sender.GetType().Name}] {message}")
		{
			
		}
	}
}