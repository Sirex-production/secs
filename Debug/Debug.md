# Debug Profiler extension

Debug Profiler previews [world](../README.md#world), [systems](../README.md#systems) and allows to modify entity's [components](../README.md#component)

## Content

- [Setup](#setup)

## Setup

1. Make sure you have Debug extension installed by checking if you have `Debug/` folder inside `secs/`.
2. Inside your setup script add `AttachProfiler()` method to your EcsSystems to have a preview of it.
3. Inside your setup scipt remember to add `ReleaseProfiler()` method to the same EcsSystem when the system is not longer in use.

```csharp
private void Awake()
{
	_world = new EcsWorld();
	_updateSystems = new EcsSystems(_world);
	
	//Attack profiler
	_updateSystems.AttachProfiler() 

	//Add your systems
	_updateSystems.Add(new SpawnPlayerSystem());
	_updateSystems.Add(new ReceiveInputSystem());
	_updateSystems.Add(new MovePlayerSystem());
}


private void OnDestroy()
{
	_updateSystems.ReleaseProfiler();
}

```

Also notice that `AttachProfiler()` method should be called before spawning a new Entity!!!

4. Now, after running the game you should see new Game Object (check: DontDestroyOnLoad) called Profiler that stores each world with its entities.
5. Try to modify the entity, notice that the change directly affect the entity.


