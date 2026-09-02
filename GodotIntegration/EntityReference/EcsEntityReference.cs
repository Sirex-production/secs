using Godot;

namespace Secs
{
	/// <summary>
	/// Node that holds reference to ECS entity and world
	/// </summary>
	/// <remarks>Use this node to retrieve information about entity that is attached to the node</remarks>
	public sealed partial class EcsEntityReference : Node
	{
		private int _entity = -1;
		private EcsWorld _world;

		/// <summary>
		/// Attached entity id
		/// </summary>
		public int Entity => _entity;

		/// <summary>
		/// Attached world to which entity belongs
		/// </summary>
		public EcsWorld World => _world;

		public override void _ExitTree()
		{
			if(_entity != -1)
				Unlink();
		}

		/// <summary>
		/// Links given Ecs world and entity to the EcsEntityReference
		/// </summary>
		/// <param name="world">World where entity exists</param>
		/// <param name="entity">Entity that will be attached</param>
		public void Link(EcsWorld world, int entity)
		{
			if(_entity != -1)
			{
				GD.PushError($"Trying to override entity id on already linked {nameof(EcsEntityReference)} on node {Name}");
				return;
			}

			_world = world;
			_entity = entity;
			_world.OnEntityDeleted += OnEntityDeleted;
		}

		/// <summary>
		/// Removes link to entity
		/// </summary>
		public void Unlink()
		{
			if(_entity == -1)
			{
				GD.PushError($"Link entity before unlinking it on node {Name}");
				return;
			}

			_world.OnEntityDeleted -= OnEntityDeleted;
			_world = null;
			_entity = -1;
		}

		private void OnEntityDeleted(int deletedEntity)
		{
			if(_entity != deletedEntity)
				return;

			Unlink();
		}
	}
}
