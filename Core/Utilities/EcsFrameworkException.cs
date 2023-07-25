namespace Secs
{
	public sealed class EcsFrameworkException : EcsException
	{
		private const string USER_MESSAGE = "!!! This is a ECS framework error, means there is a bug in the framework. Please report it to the developers !!!";
		
		public EcsFrameworkException(object sender, object message) : base(sender, $"{message}\n{USER_MESSAGE}")
		{
			
		}
	}
}