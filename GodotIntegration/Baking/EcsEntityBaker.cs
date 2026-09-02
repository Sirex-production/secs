using Godot;

namespace Secs
{
	/// <summary>
	/// Base class for defining baking logic for ECS
	/// </summary>
	public abstract partial class EcsEntityBaker : Node
	{
		[ExportCategory("Bake options")]
		[Export] internal string _worldId;
		[Export] internal BakeOption _bakeOption = BakeOption.BakeAndRemoveBakerNode;

		[ExportCategory("Entity reference")]
		[Export] internal bool _assignEntityReference = false;
		[Export] internal NodePath _entityReferencePath;

		/// <summary>
		/// Is fired when node baking is requested on particular tree. Override this method to define your own baking logic
		/// </summary>
		/// <param name="world">World to which entity belongs</param>
		/// <param name="entity">Created entity that should be used for baking</param>
		public abstract void Bake(EcsWorld world, int entity);

		internal enum BakeOption
		{
			BakeAndKeepBakerNode = 0,
			BakeAndRemoveBakerNode = 1,
			DontBake = 2
		}
	}
}
