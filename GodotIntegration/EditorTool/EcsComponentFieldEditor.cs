#if TOOLS
using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Secs.Debug
{
	/// <summary>
	/// Single field or property of a component that can be edited by the inspector tool
	/// </summary>
	public sealed class EcsMember
	{
		private readonly FieldInfo _field;
		private readonly PropertyInfo _property;

		public string Name => _field?.Name ?? _property.Name;
		public Type Type => _field?.FieldType ?? _property.PropertyType;

		private EcsMember(FieldInfo field)
		{
			_field = field;
		}

		private EcsMember(PropertyInfo property)
		{
			_property = property;
		}

		public object GetValue(object target)
		{
			return _field != null ? _field.GetValue(target) : _property.GetValue(target);
		}

		public void SetValue(object target, object value)
		{
			if(_field != null)
				_field.SetValue(target, value);
			else
				_property.SetValue(target, value);
		}

		/// <summary>
		/// Returns editable public members of the given type. Public fields are preferred, public settable properties are used as fallback
		/// </summary>
		public static List<EcsMember> GetMembers(Type type)
		{
			var members = new List<EcsMember>();

			foreach(var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
				members.Add(new EcsMember(field));

			if(members.Count == 0)
			{
				foreach(var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				{
					if(property.CanRead && property.CanWrite && property.GetIndexParameters().Length == 0)
						members.Add(new EcsMember(property));
				}
			}

			return members;
		}
	}

	/// <summary>
	/// Editor control bound to a single field or property of a component
	/// </summary>
	public sealed class EcsComponentFieldEditor
	{
		private readonly Action<object> _applyValue;

		/// <summary>
		/// Control that displays the edited value
		/// </summary>
		public Control Control { get; }

		private EcsComponentFieldEditor(Control control, Action<object> applyValue)
		{
			Control = control;
			_applyValue = applyValue;
		}

		/// <summary>
		/// Applies external value to the editor control. Skipped if the control is currently focused
		/// </summary>
		public void Apply(object value)
		{
			_applyValue(value);
		}

		/// <summary>
		/// Creates editor for the field with the given type and value
		/// </summary>
		/// <param name="label">Label of the field</param>
		/// <param name="fieldType">Type of the edited value</param>
		/// <param name="currentValue">Boxed current value</param>
		/// <param name="onChanged">Fired when user changes the value. Receives boxed new value</param>
		public static EcsComponentFieldEditor Create(string label, Type fieldType, object currentValue, Action<object> onChanged)
		{
			if(fieldType == typeof(bool))
			{
				var checkBox = new CheckBox { Text = label, ButtonPressed = (bool)currentValue };
				checkBox.Toggled += pressed => onChanged(pressed);
				return new EcsComponentFieldEditor(checkBox, value =>
				{
					if(!checkBox.HasFocus())
						checkBox.ButtonPressed = (bool)value;
				});
			}

			if(fieldType == typeof(string))
			{
				var lineEdit = new LineEdit { Text = (string)currentValue };
				lineEdit.TextChanged += text => onChanged(text);
				return new EcsComponentFieldEditor(lineEdit, value =>
				{
					if(!lineEdit.HasFocus())
						lineEdit.Text = (string)value;
				});
			}

			if(IsInteger(fieldType))
			{
				GetIntegerRange(fieldType, out var min, out var max);

				var spinBox = CreateSpinBox(1.0, min, max, allowOutOfRange: false);
				spinBox.Value = Convert.ToDouble(currentValue);
				spinBox.ValueChanged += value => onChanged(ConvertToInteger(value, fieldType));
				return new EcsComponentFieldEditor(spinBox, value =>
				{
					if(!IsSpinBoxFocused(spinBox))
						spinBox.Value = Convert.ToDouble(value);
				});
			}

			if(fieldType == typeof(float) || fieldType == typeof(double))
			{
				var spinBox = CreateSpinBox(0.01, -1000000000, 1000000000, allowOutOfRange: true);
				spinBox.Value = Convert.ToDouble(currentValue);
				spinBox.ValueChanged += value => onChanged(fieldType == typeof(float) ? (object)(float)value : value);
				return new EcsComponentFieldEditor(spinBox, value =>
				{
					if(!IsSpinBoxFocused(spinBox))
						spinBox.Value = Convert.ToDouble(value);
				});
			}

			if(fieldType.IsEnum)
			{
				var enumValues = Enum.GetValues(fieldType);
				var optionButton = new OptionButton();
				foreach(var enumName in Enum.GetNames(fieldType))
					optionButton.AddItem(enumName);

				optionButton.Select(Array.IndexOf(enumValues, currentValue));
				optionButton.ItemSelected += index => onChanged(enumValues.GetValue((int)index));

				return new EcsComponentFieldEditor(optionButton, value =>
				{
					if(!optionButton.HasFocus())
						optionButton.Select(Array.IndexOf(enumValues, value));
				});
			}

			if(fieldType.IsValueType)
			{
				var container = new VBoxContainer();
				var members = EcsMember.GetMembers(fieldType);

				// Boxed struct that write callbacks mutate; kept in sync with the outside world by Apply()
				// on every refresh so a write never clobbers a sibling field that changed concurrently.
				var liveValue = currentValue;

				var childEditors = new List<(EcsMember Member, EcsComponentFieldEditor Editor)>();
				foreach(var member in members)
				{
					var editor = Create(member.Name, member.Type, member.GetValue(liveValue), newValue =>
					{
						member.SetValue(liveValue, newValue);
						onChanged(liveValue);
					});

					childEditors.Add((member, editor));
					container.AddChild(editor.Control);
				}

				return new EcsComponentFieldEditor(container, value =>
				{
					liveValue = value;
					foreach(var (member, editor) in childEditors)
						editor.Apply(member.GetValue(liveValue));
				});
			}

			var labelControl = new Label { Text = $"{label}: {currentValue}" };
			return new EcsComponentFieldEditor(labelControl, _ => { });
		}

		private static bool IsInteger(Type type)
		{
			return type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong) ||
				type == typeof(short) || type == typeof(ushort) || type == typeof(byte) || type == typeof(sbyte);
		}

		private static bool IsSpinBoxFocused(SpinBox spinBox)
		{
			return spinBox.HasFocus() || spinBox.GetLineEdit().HasFocus();
		}

		private static SpinBox CreateSpinBox(double step, double min, double max, bool allowOutOfRange)
		{
			return new SpinBox
			{
				MinValue = min,
				MaxValue = max,
				Step = step,
				AllowGreater = allowOutOfRange,
				AllowLesser = allowOutOfRange
			};
		}

		private static void GetIntegerRange(Type type, out double min, out double max)
		{
			if(type == typeof(byte)) { min = byte.MinValue; max = byte.MaxValue; return; }
			if(type == typeof(sbyte)) { min = sbyte.MinValue; max = sbyte.MaxValue; return; }
			if(type == typeof(short)) { min = short.MinValue; max = short.MaxValue; return; }
			if(type == typeof(ushort)) { min = ushort.MinValue; max = ushort.MaxValue; return; }
			if(type == typeof(uint)) { min = uint.MinValue; max = uint.MaxValue; return; }
			if(type == typeof(ulong)) { min = 0; max = long.MaxValue; return; }
			if(type == typeof(long)) { min = long.MinValue; max = long.MaxValue; return; }

			min = int.MinValue;
			max = int.MaxValue;
		}

		// Clamps before converting so a SpinBox value at the edge of a type's range (where the
		// double representation can round outside the type's actual bounds) never throws OverflowException.
		private static object ConvertToInteger(double value, Type fieldType)
		{
			if(fieldType == typeof(byte)) return (byte)Math.Clamp(value, byte.MinValue, byte.MaxValue);
			if(fieldType == typeof(sbyte)) return (sbyte)Math.Clamp(value, sbyte.MinValue, sbyte.MaxValue);
			if(fieldType == typeof(short)) return (short)Math.Clamp(value, short.MinValue, short.MaxValue);
			if(fieldType == typeof(ushort)) return (ushort)Math.Clamp(value, ushort.MinValue, ushort.MaxValue);
			if(fieldType == typeof(uint)) return (uint)Math.Clamp(value, uint.MinValue, uint.MaxValue);
			if(fieldType == typeof(ulong)) return (ulong)Math.Clamp(value, 0, long.MaxValue);
			if(fieldType == typeof(long)) return (long)Math.Clamp(value, long.MinValue, long.MaxValue);

			return (int)Math.Clamp(value, int.MinValue, int.MaxValue);
		}
	}
}
#endif
