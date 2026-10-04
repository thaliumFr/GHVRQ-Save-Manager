using System.Diagnostics.CodeAnalysis;
using UraniumUI.Material.Controls;

using CheckBox = UraniumUI.Material.Controls.CheckBox;

namespace GHVRQ_Save_Manager.Data
{
    public interface IUniversalInputFieldData
    {
        object Field { get; set; }
        object Value { get; set; }
    }

    public interface IUniversalInputFieldData<TField, TValue> : IUniversalInputFieldData
    {
        new TField Field { get; set; }
        new TValue Value { get; set; }
    }

    public struct TextFieldData : IUniversalInputFieldData<TextField, string>
    {
        public TextField Field { get; set; }
        public readonly string Value { get { return Field.Text; } set { Field.Text = value; } }
        object IUniversalInputFieldData.Field { get => Field; set => Field = (TextField)value; }
        object IUniversalInputFieldData.Value { get => Value; set => Value = (string)value; }

        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is TextFieldData other && Field == other.Field;
        }

        public readonly override int GetHashCode()
        {
            return Field?.GetHashCode() ?? 0;
        }
    }

    public struct ToggleFieldData : IUniversalInputFieldData<CheckBox, bool>
    {
        public CheckBox Field { get; set; }
        public readonly bool Value { get { return Field.IsChecked; } set { Field.IsChecked = value; } }
        object IUniversalInputFieldData.Field { get => Field; set => Field = (CheckBox)value; }
        object IUniversalInputFieldData.Value { get => Value; set => Value = bool.Parse((string)value); }

        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is ToggleFieldData other && Field == other.Field;
        }

        public readonly override int GetHashCode()
        {
            return Field?.GetHashCode() ?? 0;
        }
    }
}
