# Non generic pools

Sometime you can find yourself struggling with generics and early binding when working with pools.
For example you need to get a pool of a specific type, but you don't know the type at compile time:

```csharp
public void AddAllComponentsToTheEntity(EcsWorld world, int entity, Type[] someCmpTypes)
{
    foreach(var type in someCmpTypes)
    {
        object pool = world.GetPool(type);
        pool.AddComponent(entity); //it is not possible with generics as type of pool is not defined 
    }
}
```

So in order to solve that problem this module was created. So to achieve same result as above you can do:

```csharp
public void AddAllComponentsToTheEntity(EcsWorld world, int entity, Type[] someCmpTypes)
{
    foreach(var type in someCmpTypes)
    {
        IEcsPoolNonGeneric pool = world.GetNonGenericPool(type);
        pool.AddComponentVirtual(entity); 
    }
}
```

All usual functionality that is provided by the generic pool by default is also available in the non generic pool:

```csharp
int entity = world.NewEntity();
IEcsPoolNonGeneric pool = world.GetNonGenericPool(type);
var component = new SomeCmp 
{
    value = 1
};

pool.AddComponentVirtual(entity, component);
var componentCopy = pool.GetComponentCopyVirtual(entity);
componentCopy.value = 2;

pool.SetComponentVirtual(entity, componentCopy);

pool.DelComponentVirtual(entity);

bool hasSomeCmp = pool.HasComponentVirtual(entity);
```

## Performance
Note that working with non generic pools is slower than working with generic pools, so for best performance it is 
recommended to avoid using non generic pools when possible.

For your convenience methods names in non generic pools are suffixed with "Virtual" to mark their "virtual call" nature.