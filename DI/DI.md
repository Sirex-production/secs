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
   - `[EcsInject]` - marks `EcsPool<TComponent>`, `EcsWorld` fields that will be injected
   - `[EcsInject(typeof(CmpX))]` and `[AndExclude(typeof(CmpY))]` - marks and defines filtering rules for `EcsFilter` fields that will be injected. Take a look on [Detailed explanation on injecting **EcsFilter**](#detailed-explanation-on-injecting-ecsfilter) for more info
4. That's it! Good job!

### Example:
```csharp
public sealed class MovePlayerSystem : IEcsRunSystem
{
    [EcsInject]
    private readonly EcsWorld _world;
    
    [EcsInject]
    private readonly EcsPool<PlayerComponent> _playerComponentPool;

    [EcsInject(typeof(PlayerComponent), typeof(TransformModel))]
    [AndExclude(typeof(IsDeadTag))]
    private readonly EcsFilter _playerFilter;

    public void OnRun()
    {
    	//Move player code
    }
}
```

### Detailed explanation on injecting `EcsFilter`

1. Add `[EcsInject(typeof(Cmp1), typeof(Cmp2), typeof(CmpN))]` attribute where `Cmp1`, `Cmp2`, `CmpN` types that you want to **include** to the filter. This attribute is required
2. Optionally add `[EcsExclude(typeof(Cmp1), typeof(Cmp2), typeof(CmpN))]` attribute where `Cmp1`, `Cmp2`, `CmpN` types that you want to **exclude**.

#### Example
```csharp
public sealed class MovePlayerSystem : IEcsRunSystem
{
    [EcsInject(typeof(PlayerComponent), typeof(TransformModel))]
    [AndExclude(typeof(IsDeadTag))]
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