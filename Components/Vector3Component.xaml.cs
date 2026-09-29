using GHVRQ_Save_Manager.XML;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;
using UraniumUI.Material.Controls;

namespace GHVRQ_Save_Manager.Components;

public partial class Vector3Component : ContentView
{
	public Vector3Component()
	{
		InitializeComponent();


	}

    public static readonly BindableProperty XMLElementProperty = BindableProperty.Create(
        nameof(XMLElement),
        typeof(XElement),
        typeof(Vector3Component),
        null);

    public XElement XMLElement
    {
        get => (XElement)GetValue(XMLElementProperty);
        set => SetValue(XMLElementProperty, value);
    }

    public static readonly BindableProperty vector3Property = BindableProperty.Create(
        nameof(XMLElement),
        typeof(Vector3),
        typeof(Vector3Component),
        null);

    public Vector3 vector3
    {
        get => (Vector3)GetValue(vector3Property);
        set => SetValue(vector3Property, value);
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

        XMLReaderSystem.AddElementLink(textField, el);
    }
}