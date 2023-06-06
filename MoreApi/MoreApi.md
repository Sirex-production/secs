# More API

**More API** is a collection of useful shortcuts that are not included in the **Core** module.
This module will reduce amount of code you need to write to achieve some common tasks.

## More EcsWorld functions

`EcsWorld` received more functions that will simplify your work with [pools](../README.md#pool) and [filters](../README.md#filter).

**Disclaimer:** most of these methods are just shortcuts for existing functionality. Usually they are a little bit slower then using original classical approach. 
They are just more convenient to use. We prefer to use them in not frequent operations. 

All recommendations on using specific shortcut are written in the description of the shortcut in a form of documentation comments. 

```csharp
ref var playerCmp = ref world.AddCmp<PlayerCmp>(entity);
```

```csharp
ref var playerCmp = ref world.GetCmp<PlayerCmp>(entity);
```

```csharp
bool hasPlayer = world.HasCmp<PlayerCmp>(entity);
```

```csharp
world.DelCmp<PlayerCmp>(entity);
```

```csharp
ref var playerCmp = ref world.NewEntityWithCmp<PlayerCmp>(out int createdEntity);
```

```csharp
world.TryGetFirstEntityWithCmp<PlayerCmp>(out int foundEntity);
```

```csharp
world.TryGetFirstCmp(out PlayerCmp playerCmp);
```

```csharp
world.TryGetFirstCmp(out PlayerCmp playerCmp, out int entityWithPlayerCmp);
```

## More EcsFilter functions

```csharp
if(!filter.IsEmpty)
	filter.GetFirstEntity();
```
