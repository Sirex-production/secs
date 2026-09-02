#if TOOLS
using Godot;

namespace Secs.Debug
{
	public sealed partial class SecsEcsToolPlugin : EditorPlugin
	{
		private EcsToolDock _dock;

		public override void _EnterTree()
		{
			_dock = new EcsToolDock { Name = "SECS ECS Inspector" };
			AddControlToDock(DockSlot.RightUl, _dock);
		}

		public override void _ExitTree()
		{
			if(_dock == null)
				return;

			RemoveControlFromDocks(_dock);
			_dock.QueueFree();
			_dock = null;
		}
	}
}
#endif
