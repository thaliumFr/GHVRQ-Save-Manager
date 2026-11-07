using GHVRQ_Save_Manager.XML;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using UraniumUI.Material.Controls;
using System.Diagnostics;

using CheckBox = UraniumUI.Material.Controls.CheckBox;
using UraniumUI.Controls;

namespace GHVRQ_Save_Manager.Data
{
    public interface IUniversalInputFieldData
    {
        object Field { get; set; }
        object Value { get; set; }

        public static void AddToList(TextField textField, XElement XMLElement)
        {
            if (String.IsNullOrEmpty(textField.ReturnCommandParameter?.ToString()))
            {
                Debug.WriteLine("ReturnCommandParameter is null or empty.");
                return;
            }

            XElement? el = XMLElement;

            if (!(bool)textField.ReturnCommandParameter.ToString()?.Contains(XMLElement.Name.LocalName))
            {
                el = XMLElement.Element(textField.ReturnCommandParameter.ToString());
                if (el == null)
                {
                    Debug.WriteLine($"Element for {XMLElement.Name}.{textField.ReturnCommandParameter} not found.");
                    return;
                }
            }

            XMLReaderSystem.AddElementLink(new TextFieldData() { Field = textField }, el);
        }

        public static void AddToList(CheckBox checkBox, XElement XMLElement)
        {
            if (String.IsNullOrEmpty(checkBox.CommandParameter?.ToString()))
            {
                Debug.WriteLine("ReturnCommandParameter is null or empty.");
                return;
            }

            XElement? el = XMLElement;
            if (!(bool)checkBox.CommandParameter.ToString()?.Contains(XMLElement.Name.LocalName))
            {
                el = XMLElement.Element(checkBox.CommandParameter?.ToString());
                if (el == null)
                {
                    Debug.WriteLine($"Element for {XMLElement.Name}.{checkBox.CommandParameter} not found.");
                    return;
                }
            }

            XMLReaderSystem.AddElementLink(new ToggleFieldData() { Field = checkBox }, el);
        }

        public static void AddToList(Dropdown dropdown, XElement XMLElement)
        {
            if (String.IsNullOrEmpty(dropdown.CommandParameter?.ToString()))
            {
                Debug.WriteLine("ReturnCommandParameter is null or empty.");
                return;
            }

            XElement? el = XMLElement;
            if (!(bool)dropdown.CommandParameter.ToString()?.Contains(XMLElement.Name.LocalName))
            {
                el = XMLElement.Element(dropdown.CommandParameter?.ToString());
                if (el == null)
                {
                    Debug.WriteLine($"Element for {XMLElement.Name}.{dropdown.CommandParameter} not found.");
                    return;
                }
            }

            XMLReaderSystem.AddElementLink(new DropdownFieldData() { Field = dropdown }, el);
        }

        public static void AddToList(object sender, XElement XMLElement)
        {
            if (sender is TextField textField)
            {
                AddToList(textField, XMLElement);
            }
            else if (sender is CheckBox checkBox)
            {
                AddToList(checkBox, XMLElement);
            }
            else if (sender is Dropdown dropdown)
            {
                AddToList(dropdown, XMLElement);
            }
            else
            {
                Debug.WriteLine($"Unsupported sender type: {sender.GetType().Name}");
            }
        }

        public static void AddToList(object sender, string path)
        {
            XElement XMLElement = XMLReaderSystem.GetElementFromPath(path);
            AddToList(sender, XMLElement);
        }
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

    public struct DropdownFieldData : IUniversalInputFieldData<Dropdown, string>
    {
        public Dropdown Field { get; set; }
        public readonly string Value { get { return Field.SelectedItem?.ToString() ?? string.Empty; } set { Field.SelectedItem = value; } }
        object IUniversalInputFieldData.Field { get => Field; set => Field = (Dropdown)value; }
        object IUniversalInputFieldData.Value { get => Value; set => Value = (string)value; }

        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is DropdownFieldData other && Field == other.Field;
        }

        public readonly override int GetHashCode()
        {
            return Field?.GetHashCode() ?? 0;
        }
    }
}
