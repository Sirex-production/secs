using Godot;

namespace Secs
{
	public static class NodeExtensions
	{
		private const string ReferenceNodeName = nameof(EcsEntityReference);

		/// <summary>
		/// Links entity to the EcsEntityReference child node of the given node
		/// </summary>
		/// <param name="node">Node to which reference will be attached</param>
		/// <param name="world">World to which entity belongs</param>
		/// <param name="entityId">Entity that will be attached</param>
		public static void LinkEcsEntity(this Node node, EcsWorld world, int entityId)
		{
			var reference = node.GetNodeOrNull<EcsEntityReference>(ReferenceNodeName);

			if(reference == null)
			{
				reference = new EcsEntityReference { Name = ReferenceNodeName };
				node.AddChild(reference);
			}

			reference.Link(world, entityId);
		}

		/// <summary>
		/// Unlinks entity from the EcsEntityReference child node of the given node
		/// </summary>
		/// <param name="node">Node from which reference will be unlinked</param>
		public static void UnlinkEcsEntity(this Node node)
		{
			var reference = node.GetNodeOrNull<EcsEntityReference>(ReferenceNodeName);

			if(reference == null)
			{
				GD.PushError($"Node {node.Name} must have {nameof(EcsEntityReference)} before unlinking it");
				return;
			}

			reference.Unlink();
		}
	}
}
