#if TOOLS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace Secs.Debug
{
	[Tool]
	public sealed partial class EcsToolDock : HSplitContainer
	{
		private const double RefreshInterval = 0.2;

		private readonly Tree _tree = new();
		private readonly ScrollContainer _inspectorScroll = new();
		private readonly VBoxContainer _inspector = new();
		private readonly Label _emptyLabel = new();
		private readonly Timer _refreshTimer = new();

		private readonly Dictionary<EcsWorld, TreeItem> _worldItems = new();
		private readonly Dictionary<EcsWorld, TreeItem> _systemsGroupItems = new();
		private readonly Dictionary<EcsWorld, int> _systemsCounts = new();
		private readonly Dictionary<EcsWorld, TreeItem> _singletonGroupItems = new();
		private readonly Dictionary<EcsWorld, HashSet<Type>> _singletonTypes = new();
		private readonly Dictionary<EcsWorld, Dictionary<int, TreeItem>> _entityItems = new();

		private readonly Dictionary<TreeItem, EcsWorld> _worldByItem = new();
		private readonly Dictionary<TreeItem, int> _entityByItem = new();
		private readonly Dictionary<TreeItem, Type> _componentByItem = new();
		private readonly Dictionary<TreeItem, Type> _singletonByItem = new();

		private EcsWorld _selectedWorld;
		private int _selectedEntity = -1;
		private Type _selectedComponent;
		private Type _selectedSingleton;
		private object _liveValue;
		private readonly List<(EcsMember Member, EcsComponentFieldEditor Editor)> _fieldBindings = new();

		private static List<Type> _allComponentTypes;
		private static List<Type> _allSingletonTypes;

		public override void _Ready()
		{
			SplitOffsets = new[] { 320 };

			_tree.HideRoot = true;
			_tree.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			_tree.CustomMinimumSize = new Vector2(200, 0);
			_tree.ItemSelected += RebuildInspector;

			_inspectorScroll.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			_inspectorScroll.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
			_inspector.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			_inspectorScroll.AddChild(_inspector);

			_emptyLabel.Text = "No worlds observed.\nCall EcsSystems.AttachObserver() to observe a world.";
			_emptyLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			_inspector.AddChild(_emptyLabel);

			AddChild(_tree);
			AddChild(_inspectorScroll);

			_refreshTimer.WaitTime = RefreshInterval;
			_refreshTimer.Autostart = true;
			_refreshTimer.Timeout += Refresh;
			AddChild(_refreshTimer);

			Refresh();
		}

		private void Refresh()
		{
			RefreshWorlds();
			RefreshInspectorValues();
		}

		private void RefreshWorlds()
		{
			var worlds = new List<EcsWorld>(EcsWorldRegistry.Worlds.Keys);

			var staleWorlds = new List<EcsWorld>();
			foreach(var worldItem in _worldItems)
			{
				if(!worlds.Contains(worldItem.Key))
					staleWorlds.Add(worldItem.Key);
			}

			foreach(var world in staleWorlds)
			{
				PurgeAndFree(_worldItems[world]);
				_worldItems.Remove(world);
				_systemsGroupItems.Remove(world);
				_systemsCounts.Remove(world);
				_singletonGroupItems.Remove(world);
				_singletonTypes.Remove(world);
				_entityItems.Remove(world);
			}

			foreach(var world in worlds)
			{
				if(!_worldItems.ContainsKey(world))
					CreateWorldItem(world);

				RefreshSystems(world);
				RefreshSingletons(world);
				RefreshEntities(world);
			}
		}

		private void CreateWorldItem(EcsWorld world)
		{
			var worldItem = _tree.CreateItem();
			worldItem.SetText(0, $"World {world.Id}");
			_worldItems.Add(world, worldItem);
			_worldByItem.Add(worldItem, world);
			_systemsCounts.Add(world, -1);

			var singletonGroupItem = worldItem.CreateChild();
			singletonGroupItem.SetText(0, "Singleton components");
			_singletonGroupItems.Add(world, singletonGroupItem);
			_singletonTypes.Add(world, new HashSet<Type>());
		}

		private void RefreshSystems(EcsWorld world)
		{
			EcsWorldRegistry.Worlds.TryGetValue(world, out var systemsList);

			var totalSystems = 0;
			if(systemsList != null)
			{
				foreach(var systems in systemsList)
					totalSystems += systems.AllSystems.Count;
			}

			if(_systemsCounts[world] == totalSystems)
				return;

			_systemsCounts[world] = totalSystems;

			if(_systemsGroupItems.TryGetValue(world, out var existingItem))
			{
				PurgeAndFree(existingItem);
				_systemsGroupItems.Remove(world);
			}

			if(totalSystems == 0)
				return;

			// Inserted at index 0 so it stays the first child, matching the original ordering.
			var systemsItem = _worldItems[world].CreateChild(0);
			systemsItem.SetText(0, $"Systems ({systemsList.Count})");
			_systemsGroupItems.Add(world, systemsItem);

			foreach(var systems in systemsList)
			{
				foreach(var system in systems.AllSystems)
				{
					var systemItem = systemsItem.CreateChild();
					systemItem.SetText(0, system.GetType().Name);
				}
			}
		}

		private void RefreshSingletons(EcsWorld world)
		{
			var presentTypes = new HashSet<Type>();
			foreach(var type in GetAllSingletonTypes())
			{
				if(world.IsSingletonPresent(type))
					presentTypes.Add(type);
			}

			var cachedTypes = _singletonTypes[world];
			if(cachedTypes.SetEquals(presentTypes))
				return;

			var groupItem = _singletonGroupItems[world];
			foreach(var child in groupItem.GetChildren())
			{
				_singletonByItem.Remove(child);
				child.Free();
			}

			foreach(var type in presentTypes)
			{
				var item = groupItem.CreateChild();
				item.SetText(0, type.Name);
				_singletonByItem.Add(item, type);
			}

			cachedTypes.Clear();
			cachedTypes.UnionWith(presentTypes);
		}

		private void RefreshEntities(EcsWorld world)
		{
			var worldItem = _worldItems[world];

			if(!_entityItems.TryGetValue(world, out var entityDict))
			{
				entityDict = new Dictionary<int, TreeItem>();
				_entityItems.Add(world, entityDict);
			}

			var alive = new HashSet<int>(world.AliveEntities);

			var staleEntities = new List<int>();
			foreach(var pair in entityDict)
			{
				if(!alive.Contains(pair.Key))
					staleEntities.Add(pair.Key);
			}

			foreach(var entity in staleEntities)
			{
				PurgeAndFree(entityDict[entity]);
				entityDict.Remove(entity);
			}

			foreach(var entity in alive)
			{
				if(!entityDict.TryGetValue(entity, out var entityItem))
				{
					entityItem = worldItem.CreateChild();
					entityDict.Add(entity, entityItem);
					_entityByItem.Add(entityItem, entity);
				}

				entityItem.SetText(0, BuildEntityLabel(world, entity));
				RefreshEntityComponents(world, entity, entityItem);
			}
		}

		private void RefreshEntityComponents(EcsWorld world, int entity, TreeItem entityItem)
		{
			var componentTypes = new HashSet<Type>(world.GetEntityComponentsTypeMask(entity).GetComponents());

			foreach(var child in entityItem.GetChildren())
			{
				if(!_componentByItem.TryGetValue(child, out var childType))
					continue;

				if(componentTypes.Remove(childType))
					continue;

				_componentByItem.Remove(child);
				child.Free();
			}

			foreach(var type in componentTypes)
			{
				var item = entityItem.CreateChild();
				item.SetText(0, type.Name);
				_componentByItem.Add(item, type);
			}
		}

		private static string BuildEntityLabel(EcsWorld world, int entity)
		{
			var sb = new StringBuilder($"Entity {entity}: ");

			foreach(var type in world.GetEntityComponentsTypeMask(entity).GetComponents())
				sb.Append(type.Name).Append(' ');

			return sb.ToString();
		}

		private void RebuildInspector()
		{
			var selected = _tree.GetSelected();
			if(selected == null)
			{
				ClearInspector();
				return;
			}

			if(_componentByItem.TryGetValue(selected, out var componentType) && selected.GetParent() != null && selected.GetParent().GetParent() != null)
			{
				_selectedWorld = _worldByItem[selected.GetParent().GetParent()];
				_selectedEntity = _entityByItem[selected.GetParent()];
				_selectedComponent = componentType;
				_selectedSingleton = null;
				BuildComponentInspector();
				return;
			}

			if(_singletonByItem.TryGetValue(selected, out var singletonType) && selected.GetParent() != null && selected.GetParent().GetParent() != null)
			{
				_selectedWorld = _worldByItem[selected.GetParent().GetParent()];
				_selectedEntity = -1;
				_selectedComponent = null;
				_selectedSingleton = singletonType;
				BuildSingletonInspector();
				return;
			}

			if(_entityByItem.TryGetValue(selected, out var entity) && selected.GetParent() != null)
			{
				_selectedWorld = _worldByItem[selected.GetParent()];
				_selectedEntity = entity;
				_selectedComponent = null;
				_selectedSingleton = null;
				BuildEntityInspector();
				return;
			}

			ClearInspector();
		}

		private void ClearInspector()
		{
			_selectedEntity = -1;
			_selectedComponent = null;
			_selectedSingleton = null;
			_liveValue = null;
			_fieldBindings.Clear();

			foreach(var child in _inspector.GetChildren())
			{
				if(child == _emptyLabel)
					continue;

				child.QueueFree();
			}

			_emptyLabel.Visible = true;
		}

		private void BuildComponentInspector()
		{
			_emptyLabel.Visible = false;

			var header = CreateHeaderLabel($"{_selectedComponent.Name} on Entity {_selectedEntity}");
			_inspector.AddChild(header);

			_inspector.AddChild(CreateDeleteButton($"Delete {_selectedComponent.Name}", () =>
			{
				if(!_selectedWorld.AliveEntities.Contains(_selectedEntity) || !_selectedWorld.HasItem(_selectedComponent, _selectedEntity))
					return;

				_selectedWorld.DelItem(_selectedComponent, _selectedEntity);
				ClearInspector();
			}));

			_liveValue = _selectedWorld.GetItem(_selectedComponent, _selectedEntity);
			BuildFieldGrid(() => _liveValue, value =>
			{
				_liveValue = value;
				_selectedWorld.ReplaceItem(_selectedComponent, _selectedEntity, (IEcsComponent)value);
			});
		}

		private void BuildSingletonInspector()
		{
			_emptyLabel.Visible = false;

			var header = CreateHeaderLabel($"Singleton {_selectedSingleton.Name}");
			_inspector.AddChild(header);

			_inspector.AddChild(CreateDeleteButton($"Delete {_selectedSingleton.Name}", () =>
			{
				_selectedWorld.DelSingletonItem(_selectedSingleton);
				ClearInspector();
			}));

			_liveValue = _selectedWorld.GetSingletonItem(_selectedSingleton);
			BuildFieldGrid(() => _liveValue, value =>
			{
				_liveValue = value;
				_selectedWorld.SetSingletonItem(_selectedSingleton, value);
			});
		}

		private void BuildEntityInspector()
		{
			_emptyLabel.Visible = false;

			var header = CreateHeaderLabel($"Entity {_selectedEntity}");
			_inspector.AddChild(header);

			var existingTypes = new HashSet<Type>(_selectedWorld.GetEntityComponentsTypeMask(_selectedEntity).GetComponents());
			var componentTypes = GetAllComponentTypes().Where(type => !existingTypes.Contains(type)).ToList();
			if(componentTypes.Count == 0)
				return;

			var optionButton = new OptionButton();
			foreach(var type in componentTypes)
				optionButton.AddItem(type.Name);

			var addButton = new Button { Text = "Add component" };
			addButton.Pressed += () =>
			{
				var type = componentTypes[optionButton.Selected];
				var component = (IEcsComponent)Activator.CreateInstance(type);
				_selectedWorld.AddItem(type, _selectedEntity, component);
			};

			var addRow = new HBoxContainer();
			addRow.AddChild(optionButton);
			addRow.AddChild(addButton);
			_inspector.AddChild(addRow);
		}

		private void BuildFieldGrid(Func<object> getValue, Action<object> onChanged)
		{
			var grid = new GridContainer { Columns = 2 };
			grid.SizeFlagsHorizontal = SizeFlags.ExpandFill;

			foreach(var member in EcsMember.GetMembers(getValue().GetType()))
			{
				var capturedMember = member;

				var label = new Label { Text = member.Name };
				grid.AddChild(label);

				// Fetches the live value at write time (rather than closing over the snapshot taken when
				// the panel was built) so a write never clobbers a sibling field refreshed in the meantime.
				var editor = EcsComponentFieldEditor.Create(member.Name, member.Type, member.GetValue(getValue()), newValue =>
				{
					var current = getValue();
					capturedMember.SetValue(current, newValue);
					onChanged(current);
				});

				_fieldBindings.Add((member, editor));
				grid.AddChild(editor.Control);
			}

			_inspector.AddChild(grid);
		}

		private void RefreshInspectorValues()
		{
			if(_selectedEntity == -1 && _selectedSingleton == null)
				return;

			if(_selectedSingleton != null)
			{
				if(!_selectedWorld.IsSingletonPresent(_selectedSingleton))
				{
					ClearInspector();
					return;
				}

				_liveValue = _selectedWorld.GetSingletonItem(_selectedSingleton);
			}
			else
			{
				if(!_selectedWorld.AliveEntities.Contains(_selectedEntity))
				{
					ClearInspector();
					return;
				}

				// Entity-only inspector (no component selected): liveness above is all there is to refresh.
				if(_selectedComponent == null)
					return;

				if(!_selectedWorld.HasItem(_selectedComponent, _selectedEntity))
				{
					ClearInspector();
					return;
				}

				_liveValue = _selectedWorld.GetItem(_selectedComponent, _selectedEntity);
			}

			foreach(var (member, editor) in _fieldBindings)
				editor.Apply(member.GetValue(_liveValue));
		}

		private static Label CreateHeaderLabel(string text)
		{
			return new Label { Text = text, SizeFlagsHorizontal = SizeFlags.ExpandFill };
		}

		private static Button CreateDeleteButton(string text, Action onPressed)
		{
			var button = new Button { Text = text };
			button.Pressed += onPressed;
			return button;
		}

		private void PurgeAndFree(TreeItem item)
		{
			foreach(var child in item.GetChildren())
				PurgeAndFree(child);

			_worldByItem.Remove(item);
			_entityByItem.Remove(item);
			_componentByItem.Remove(item);
			_singletonByItem.Remove(item);

			item.Free();
		}

		private static List<Type> GetAllComponentTypes()
		{
			if(_allComponentTypes == null)
			{
				_allComponentTypes = AppDomain.CurrentDomain
					.GetAssemblies()
					.SelectMany(assembly => assembly.GetTypes())
					.Where(type => type.IsValueType && !type.IsAbstract && typeof(IEcsComponent).IsAssignableFrom(type) && !typeof(IEcsSingletonComponent).IsAssignableFrom(type))
					.OrderBy(type => type.Name)
					.ToList();
			}

			return _allComponentTypes;
		}

		private static List<Type> GetAllSingletonTypes()
		{
			if(_allSingletonTypes == null)
			{
				_allSingletonTypes = AppDomain.CurrentDomain
					.GetAssemblies()
					.SelectMany(assembly => assembly.GetTypes())
					.Where(type => type.IsValueType && !type.IsAbstract && typeof(IEcsSingletonComponent).IsAssignableFrom(type))
					.OrderBy(type => type.Name)
					.ToList();
			}

			return _allSingletonTypes;
		}
	}
}
#endif
