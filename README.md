# SECS (Sirex Entity Component System)

SECS is a lightweight, C# engine-independent Entity Component System framework based on `structs`

## Entity
Entity is an `int` ID that serves as a container for [components](#component). 
```csharp
// Creating entity
int entity = world.NewEntity();
```

```csharp
// Deleting entity
world.DelEntity(entity);
```

## Component
Container for holding data. Must be `struct` that implements `IEcsComponent` interface
```csharp
public struct PlayerCmp : IEcsComponent
{
    public float speed;
}
```

## World
World is a place that holds all the data about [filters](#filter), [entities](#entity) and their components. Provides API for getting [pools](#pool), [filters](#filter), adding or deleting [entities](#entity).

## Matcher
Matcher is a container that used for defining matching conditions for the [filter](#filter).
```csharp
// Only entities that have InputCmp will be present in the filter
EcsMatcher inputMatcher = EcsMatcher.Include(typeof(InputCmp)).End();
EcsFilter inputFilter = world.GetFilter(inputMatcher);

//Only alive enemies (without IsDeadCmp) that have HealthCmp will be present in the filter
EcsMatcher aliveEnemyMatcher = EcsMatcher
				.Include
				(
					typeof(HealthCmp),
					typeof(EnemyCmp)
				)
				.Exclude
				(
					typeof(IsDeadCmp)
				)
				.End();

EcsFilter aliveEnemiesFilter = world.GetFilter(aliveEnemyMatcher);
```

## Filter
Filter is a container that keeps entities always up to date that meet matcher requirements.
```csharp
public sealed class MovePlayerSystem : IEcsRunSystem
{
	private readonly EcsFilter _playerFilter;
	private readonly EcsPool<PlayerCmp> _playerPool;
	private readonly EcsPool<VelocityCmp> _velocityPool;
	
	public MovePlayerSystem(EcsWorld world)
	{
	    // Only entities with PlayerCmp and VelocityCmp will be present in the filter
		EcsMatcher playerMatcher = EcsMatcher.Include
			(
				typeof(PlayerCmp),
				typeof(VelocityCmp)
			)
			.End();
			
		_playerFilter = world.GetFilter(playerMatcher);
		
		_playerPool = world.GetPool<PlayerCmp>();
		_velocityPool = world.GetPool<VelocityCmp>();
	}
	public void OnRun()
	{
	    //Iterating through all entities that have PlayerCmp and VelocityCmp
		foreach(var playerEntity in _playerFilter)
		{
			ref PlayerCmp playerCmp = ref _playerFilter.GetComponent(playerEntity);
			ref VelocityCmp velocityCmp = ref _velocityPool.GetComponent(playerEntity);
			
			//Launching player to the space
			velocityCmp.velocity += Vector3.up * playerCmp.speed * Time.deltaTime;
		}
	}
}
```

## Pool
Is a container for specific components. Provides an API for managing entity components.
```csharp
// Getting pools form the world
EcsPool<IsHappyCmp> isHappyCmpPool = world.GetPool<IsHappyCmp>();
EcsPool<HealthCmp> healthCmpPool = world.GetPool<HealthCmp>();
EcsPool<ApplyDamageCmp> applyDamageCmpPool = world.GetPool<ApplyDamageCmp>();
EcsPool<IsHungryCmp> isHungryCmpPool = world.GetPool<IsHungryCmp>();

// Gettings components from the entity
ref HealthCmp healthCmp = ref healthCmpPool.GetComponent(playerEntity);
ref ApplyDamageCmp applyDamageCmp = ref applyDamageCmpPool.GetComponent(playerEntity);

// Mutating component's data
healthCmp.currentHealth -= applyDamageCmp.damage;

// Deleting component from the entity	
applyDamageCmpPool.DelComponent(playerCmpPool);

// Adding component to the entity
isHappyCmpPool.AddComponent(playerEntity);

if(isHungryCmpPool.HasComponent(playerEntity))
    Print("Player is hungry :(");
```

## Systems
Is a container for logic for processing filtered entities. 

````csharp
// Creating container for systems
EcsSystems updateSystems = new EcsSystems(world);

// Adding systems
updateSystems.Add(new SpawnPlayerSystem(_world))
             .Add(new ReceiveInputSystem(_world))
             .Add(new MovePlayerSystem(_world));

// Firing system logic
updateSystems.FireInitSystems();
updateSystems.FireRunSystems();
updateSystems.FireDisposeSystems();
````

### Init system
Type of systems that are meant to be executed first.
```csharp
public sealed class InitializePlayerSystem : IEcsInitSystem
{
	public void OnInit()
	{
		// Initialize logic
	}
}
```
### Run system
Type of systems that are should be executed each frame of the game. Often contains main logic for processing filtered entities.
```csharp
public sealed class MovePlayerSystem : IEcsRunSystem
{
	public void OnRun()
	{
		// Move logic
	}
}
```
### Dispose system
Type of systems that are meant to be executed last.
```csharp
public sealed class DeletePLayer : IEcsDisposeSystem
{
	public void OnDispose()
	{
		// Player deletion logic
	}
}
```
### Combined systems 
Different system types also can be combined  together
```csharp
public sealed class PlayerGodSystem : IEcsInitSystem, IEcsRunSystem, IEcsDisposeSystem
{
	public void OnInit()
	{
		
	}
	
	public void OnRun()
	{
		
	}
	
	public void OnDispose()
	{
		
	}
}
```

# Setup

## Unity example
```csharp
public sealed class EcsSetup : MonoBehaviour
{
	private EcsWorld _world;
	private EcsSystems _updateSystems;
	
	private void Awake()
	{
            // Initialize world
            _world = new EcsWorld();
            // Initialize systems container
            _updateSystems = new EcsSystems(_world);
		
            //Add systems
            _updateSystems.Add(new SpawnPlayerSystem(_world))
		          .Add(new ReceiveInputSystem(_world))
		          .Add(new MovePlayerSystem(_world));
	}
	
	private void Start()
	{
	    //Fire logic
	    _updateSystems.FireInitSystems();
	}
	
	private void Update()
	{
            //Fire logic
            _updateSystems.FireRunSystems();
	}
	
	private void OnDestroy()
	{
            //Fire logic
            _updateSystems.FireDisposeSystems();
	}
}
```


