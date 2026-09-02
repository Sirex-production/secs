using Godot;

namespace Secs
{
	public sealed partial class EcsWorld
	{
		/// <summary>
		/// Fires baking logic for specific baker node
		/// </summary>
		/// <param name="entityBaker">Baker that will be baked</param>
		public void BakeSpecificNode(EcsEntityBaker entityBaker)
		{
			if(entityBaker._bakeOption == EcsEntityBaker.BakeOption.DontBake)
				return;

			if(!string.IsNullOrEmpty(entityBaker._worldId) && entityBaker._worldId != Id)
				return;

			int entity = NewEntity();
			entityBaker.Bake(this, entity);

			EcsEntityReference entityReference = null;

			if(entityBaker._assignEntityReference)
			{
				if(entityBaker._entityReferencePath == null)
					throw new EcsException(this, $"Entity reference path is not set on baker {entityBaker.Name}");

				entityReference = entityBaker.GetNodeOrNull<EcsEntityReference>(entityBaker._entityReferencePath);

				if(entityReference == null)
					throw new EcsException(this, $"Entity reference node {entityBaker._entityReferencePath} not found");

				entityReference.Link(this, entity);
			}

			if(entityBaker._bakeOption == EcsEntityBaker.BakeOption.BakeAndRemoveBakerNode)
			{
				// If the reference node lives inside the baker's own subtree, move it out first —
				// otherwise QueueFree() below frees it too, unlinking it right after baking.
				if(entityReference != null && entityBaker.IsAncestorOf(entityReference))
					entityReference.Reparent(entityBaker.GetParent());

				entityBaker.QueueFree();
			}
		}

		/// <summary>
		/// Finds all bakers in the node tree and bakes them
		/// </summary>
		/// <param name="rootNode">Node from which baking will start. Children of the baker node are skipped since they belong to the baker</param>
		public void BakeAllNodes(Node rootNode)
		{
			if(rootNode is EcsEntityBaker rootBaker)
			{
				BakeSpecificNode(rootBaker);
				return;
			}

			foreach(Node child in rootNode.GetChildren())
				BakeAllNodes(child);
		}
	}
}
