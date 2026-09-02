# Godot integration

This module contains a set of solutions that will help you to integrate **SECS** into your Godot project.

## Installation

SECS is meant to be dropped into a Godot project as a folder (e.g. `Vendor/secs/`). Godot compiles every `.cs` file in the project into a single assembly, so `internal` members of the framework are reachable.

Requires **Godot 4.6+** — the editor tool (`EditorTool/SecsEcsToolPlugin.cs`, `EditorTool/EcsToolDock.cs`) uses the `EditorDock` docking API introduced in that version.

- Delete the modules that are not in use — `UnityIntegration/` and `Tests/` reference Unity and NUnit and will not compile in Godot.
- The `Secs.asmdef` file is ignored by Godot, you can keep it.
- The editor tool depends on the `NonGenericPools/` module — keep that folder. The tool code is wrapped in `#if TOOLS` so it is compiled only in editor builds and excluded from exported games.
- To enable the editor tool create `addons/secs_ecs_tool/` folder in your project root and copy `GodotIntegration/EditorTool/plugin.cfg` into it (the tool itself must stay in `GodotIntegration/EditorTool/`, otherwise it will be compiled twice). If SECS is not vendored at `Vendor/secs/`, adjust the `script` path in `plugin.cfg`. Then enable `SECS ECS Inspector` in Project Settings → Plugins.

## Baking

### What is baking?
When working in Godot you might find yourself in a situation when you need to create entity with ECS components that reference particular Godot nodes from the scene tree. We call this process **baking**.

### How to use our solution
Baking mechanism consists of 5 parts:

1. Inherit from `EcsEntityBaker` class.
2. Implement `Bake` method.
3. Attach your baker to a node in the scene tree.
4. Specify baking options in inspector.
5. Invoke `world.BakeAllNodes(rootNode)` after you initialize all your [systems](../README.md/#system).

### Example
```csharp
public sealed partial class CraftingSurfaceBaker : EcsEntityBaker
{
    [Export] private CraftingSurfaceNode craftingSurface = null!;
    public override void Bake(EcsWorld world, int entity)
    {
        world.Add<CraftingSurfaceTag>(entity);
        world.Add<CraftingSurfaceMdl>(entity).craftingSurface = craftingSurface;
    }
}
```
Then when you attach and configure inspector properties (steps #3 and #4) you will be able to bake your nodes by invoking `world.BakeAllNodes(rootNode)` (step #5):

```csharp
world = new EcsWorld(id: "gameplay");
ecsSystems = new EcsSystems(world);

ecsSystems.Add(new InitRecipesSys())
        .Add(new UnlockNewRecipeSys())
        .Add(new UnlockNewItemSys());

ecsSystems.Inject(); //If you are using DI module

world.BakeAllNodes(GetNode("/root/Main")); //Bake all entities in the node tree
```

### Baking options
- `World id` — if set, the baker only bakes when `world.BakeAllNodes`/`BakeSpecificNode` is called on the world whose `Id` matches. Leave empty to bake into whichever world calls it.
- `BakeAndKeepBakerNode` — bakes the node and keeps the baker in the tree.
- `BakeAndRemoveBakerNode` (default) — bakes the node and frees the baker after baking. If the entity reference node lives inside the baker's own subtree, it is moved out first so it survives.
- `DontBake` — skips the baker.
- `Assign entity reference` + `Entity reference path` — after baking links the `EcsEntityReference` node found at the given path to the created entity.

## Ecs entity reference
### What is entity reference?

When working in Godot you might find yourself in a situation when you need to reference particular ECS entity from Godot node.
The EntityReference is basically a Godot node that holds information about Ecs entity and its world inside the scene tree.

### How to use our solution

Invoke `LinkEcsEntity(EcsWorld world, int entityId)` method on any node to attach `EcsEntityReference` child node.

After that you can get the attached `EcsWorld` and entity from it:

```csharp
var attachedWorld = myNode.GetNode<EcsEntityReference>("EcsEntityReference").World;
int entityId = myNode.GetNode<EcsEntityReference>("EcsEntityReference").Entity;
```

Also if you are using [baking](#baking) module you can set `Assign entity reference` to `true` from the Godot inspector on your baker that inherits from `EcsEntityBaker`. Then `EcsEntityReference` will be linked automatically when baking process is fired.

The reference unlinks itself automatically when the node leaves the tree or the entity is deleted.

## Editor tool
### What is editor tool?

The `SECS ECS Inspector` is a Godot editor plugin (docked panel) that previews and edits ECS worlds, entities, singleton and regular components at runtime.

### How to use our solution

1. Copy `GodotIntegration/EditorTool/plugin.cfg` to `addons/secs_ecs_tool/` and enable the plugin (see [installation](#installation)).
2. Attach your systems to the inspector by invoking `AttachObserver()` after all systems are added:

```csharp
ecsSystems.Inject(); //If you are using DI module
ecsSystems.AttachObserver(); //Attach SECS ECS Inspector
```

3. Run the game — the dock shows all observed worlds with their systems, singleton components and entities. Select an entity, component or singleton to view and edit its values. Components can be added and removed at runtime.

Observation is released automatically when `ecsSystems.FireDisposeSystems()` runs; call `ReleaseObserver()` yourself only if you need to stop observing earlier.
