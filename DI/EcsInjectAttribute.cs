using System;

namespace Secs
{
	/// <summary>
	/// Attribute that marks ECS types that will be injected
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class EcsInjectAttribute : Attribute
	{
		/// <summary>
		/// EcsFilter included types
		/// </summary>
		public readonly Type[] includeTypes;

		/// <summary>
		/// Use this constructor to inject EcsWorld and EcsPool 
		/// </summary>
		public EcsInjectAttribute() { }
		
		/// <summary>
		/// Use this constructor for injecting Filters
		/// </summary>
		/// <param name="includeTypes">Types that will to be included to filter</param>
		public EcsInjectAttribute(params Type[] includeTypes)
		{
			this.includeTypes = includeTypes;
		}
	}

	
	/// <summary>
	/// Attribute that defines exclude components for filters. Must be used with EcsInjectAttribute combination
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class AndExclude : Attribute
	{
		/// <summary>
		/// EcsFilter excluded types
		/// </summary>
		public readonly Type[] excludeTypes;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="excludeTypes">Types that will to be excluded from filter</param>
		public AndExclude(params Type[] excludeTypes)
		{
			this.excludeTypes = excludeTypes;
		}
	}
}