# Unity integration

This module contains a set of solutions that will help you to integrate **SECS** into your Unity project.

## Baking

### What is baking?
When working in Unity you might find yourself in a situation when you need to create entity with ECS components that reference particular Unity components from game object.
We call this process **baking**.

For example, you have a game object with `Transform` component and you want to attach it to your ECS entity.

You can do it manually by writing something like this:
```csharp
public sealed class CraftingSurfaceBaker : EcsMonoBaker
{
    private void Awake()
    {
        var world = YourCustomWorldLocator.GetWorldWithId("gameplay");
        int entityId = world.NewEntity();
        
        world.GetPool<CraftingSurfaceTag>().AddComponent(entityId);
        world.GetPool<TransformMdl>().AddComponent(entityId).transform = transform;
    }
}
```

But then you probably will have to write a lot of additional code (e.g. `YourCustomWorldLocator`) and maintain correct execution order so for instance your [filters](../README.md/#filter) are not created after baking.

### How to use our solution
Baking mechanism consists of 5 parts:

1. Inherit from `EcsMonoBaker` class.
2. Implement `Bake` method.
3. Attach your baker to game object.
4. Specify baking properties in inspector.
5. Invoke `world.BakeAllBakersInScene()` after you initialize all your [systems](../README.md/#system).

### Example
Once again lest use our `Transform` component as in [what is baking](#what-is-baking) section.

Now you baking code should look like this:
```csharp
public sealed class CraftingSurfaceBaker : EcsMonoBaker
{
    protected override void Bake(EcsWorld world, int entityId)
    {
        world.GetPool<CraftingSurfaceTag>().AddComponent(entityId);
        world.GetPool<TransformMdl>().AddComponent(entityId).transform = transform;
    }
}
```
Then when you attach and configure inspector properties (steps #3 and #4) you will be able to bake your game object by invoking `world.BakeAllBakersInScene()` (step #5):

```csharp
world = new EcsWorld(id:"gameplay");
ecsSystems = new EcsSystems(world);

ecsSystems.Add(new InitRecipesSys())
        .Add(new UnlockNewRecipeSys())  
        .Add(new UnlockNewItemSys());
        
ecsSystems.Inject(); //If you are using DI module        

world.BakeAllBakersInScene(); //Bake all entities in scene                        
```

## Ecs entity reference
### What is entity reference?

When working in Unity you might find yourself in a situation when you need to reference particular ECS entity from Unity game object.
The EntityReference is basically Unity component that holds information about Ecs entity and it's world inside monobehaviour.

### How to use our solution

Invoke `LinkEcsEntity(EcsWorld world, int entityId)` method on any Unity component or game object to setup EcsEntityReference.

After that `EcsEntityReference` component will be attached to game object and you can get it as simply as getting `EcsEntityReference` component form monobehaviour and getting attached EcsWorld and entity from it:

```csharp
EcsWorld attachedWorld = gameObject.GetComponent<EcsEntityReference>().World;
int entityId = gameObject.GetComponent<EcsEntityReference>().EntityId;
```

Also if you are using [baking](#baking) module you can set `createEntityReference` to `true` from the Unity inspector on your baker that inherits from `EcsMonoBaker`. Then `EcsEntityReference` will be created automatically when baking process is fired