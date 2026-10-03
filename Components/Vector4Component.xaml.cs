using GHVRQ_Save_Manager.Data;
using GHVRQ_Save_Manager.XML;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;
using UraniumUI.Material.Controls;

namespace GHVRQ_Save_Manager.Components;

public partial class Vector4Component : ContentView
{
	public Vector4Component()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty XMLElementProperty = BindableProperty.Create(
        nameof(XMLElement),
        typeof(XElement),
        typeof(Vector4Component),
        null);

    public XElement XMLElement
    {
        get => (XElement)GetValue(XMLElementProperty);
        set => SetValue(XMLElementProperty, value);
    }

    public static readonly BindableProperty vector4Property = BindableProperty.Create(
        nameof(XMLElement),
        typeof(Vector4),
        typeof(Vector4Component),
        null);

    public Vector4 vector4
    {
        get => (Vector4)GetValue(vector4Property);
        set => SetValue(vector4Property, value);
    }

    private void AddFieldToList(object sender, EventArgs e)
    {
        TextField textField = (TextField)sender;
        if (String.IsNullOrEmpty(textField.ReturnCommandParameter?.ToString()))
        {
            Debug.WriteLine("ReturnCommandParameter is null or empty.");
            return;
        }

        XElement? el = XMLElement.Element("POSITION")?.Element(textField.ReturnCommandParameter.ToString());
        if (el == null)
        {
            Debug.WriteLine($"Element for {textField.ReturnCommandParameter} not found.");
            return;
        }

        XMLReaderSystem.AddElementLink(new TextFieldData() { Field = textField }, el);
    }
}