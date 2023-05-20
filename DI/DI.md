# Dependency injection (DI) extension

DI extension allows you to inject ECS types like
[pools](../README.md#pool), [filters](../README.md#filter) and [world](../README.md#world) into your system
and reduce huge amount of code.

## Content

- [Setup](#setup)
- [Detailed explanation on injecting `EcsFilter`](#detailed-explanation-on-injecting-ecsfilter)

## Setup

1. Make sure you have DI extension installed by checking if you have `DI/` folder inside `secs/`
2. Inside your setup script add `Inject()` method to your systems:
```csharp
_world = new EcsWorld();
_updateSystems = new EcsSystems(_world);

//Add your systems
_updateSystems.Add(new SpawnPlayerSystem());
_updateSystems.Add(new ReceiveInputSystem());
_updateSystems.Add(new MovePlayerSystem());

//Inject ECS fields into added systems.
_updateSystems.Inject();
```

Also notice that `Inject()` method should be called after all systems were added 

3. Add corresponding attributes inside your systems to mark injection fields:
   - `[EcsWorldInject]` - marks `EcsWorld` fields that will be injected
   - `[EcsPoolInject]` - marks `EcsPool<TComponent>` fields that will be injected
   - `[EcsFilterInject]`, `[EcsInclude(typeof(Cmp1), typeof(Cmp2))]`, `[EcsExclude(typeof(Cmp1), typeof(Cmp2))]` - marks `EcsFilter` fields that will be injected

4. That's it! Good job!

### Example:
```csharp
public sealed class MovePlayerSystem : IEcsRunSystem
{
    [EcsWorldInject]
    private readonly EcsWorld _world;
    
    [EcsPoolInject]
    private readonly EcsPool<PlayerComponent> _playerComponentPool;

    [EcsFilterInject]
    [EcsInclude(typeof(PlayerComponent), typeof(TransformModel))]
    [EcsExclude(typeof(IsDeadTag))]
    private readonly EcsFilter _playerFilter;

    public void OnRun()
    {
    	//Move player code
    }
}
```

### Detailed explanation on injecting `EcsFilter`

1. First mark filter that you want to inject with `[EcsFilterInject]` attribute. In other case injection will be ignored
2. Add `[EcsInclude(typeof(Cmp1), typeof(Cmp2), typeof(CmpN))]` attribute where `Cmp1`, `Cmp2`, `CmpN` types that you want to **include**. This attribute is also required
3. Optionally add `[EcsExclude(typeof(Cmp1), typeof(Cmp2), typeof(CmpN))]` attribute where `Cmp1`, `Cmp2`, `CmpN` types that you want to **exclude**.

#### Example
```csharp
public sealed class MovePlayerSystem : IEcsRunSystem
{
    [EcsFilterInject]
    [EcsInclude(typeof(PlayerComponent), typeof(TransformModel))]
    [EcsExclude(typeof(IsDeadTag))]
    private readonly EcsFilter _playerFilter;

    public void OnRun()
    {
    	//Move player code
    }
}
```
is same as
```csharp
public sealed class MovePlayerSystem : IEcsRunSystem
{
    private readonly EcsFilter _playerFilter;

    public MovePlayerSystem(EcsWorld world)
    {
        var matcher = EcsMatcher
            .Include(typeof(PlayerComponent), typeof(TransformModel))
            .Exclude(typeof(IsDeadTag))
            .End();

        _playerFilter = world.GetFilter(matcher); 
    }

    public void OnRun()
    {
    	//Move player code
    }
}
```