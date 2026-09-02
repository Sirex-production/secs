#if TOOLS
using Godot;

namespace Secs.Debug
{
	[Tool]
	public sealed partial class SecsEcsToolPlugin : EditorPlugin
	{
		private EditorDock _dock;

		public override void _EnterTree()
		{
			_dock = new EditorDock { Title = "SECS ECS Inspector", DefaultSlot = EditorDock.DockSlot.RightUl };
			_dock.AddChild(new EcsToolDock());
			AddDock(_dock);
		}

		public override void _ExitTree()
		{
			if(_dock == null)
				return;

			RemoveDock(_dock);
			_dock.QueueFree();
			_dock = null;
		}
	}
}
#endif
