using GHVRQ_Save_Manager.Data;
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

    public static readonly BindableProperty Vector3DataProperty = BindableProperty.Create(
        nameof(Vector3Data),
        typeof(Vector3Data),
        typeof(Vector3Component),
        null);

    public Vector3Data Vector3Data
    {
        get => (Vector3Data)GetValue(Vector3DataProperty);
        set => SetValue(Vector3DataProperty, value);
    }

    private void AddFieldToList(object sender, EventArgs e)
    {
        TextField textField = (TextField)sender;
        if (String.IsNullOrEmpty(textField.ReturnCommandParameter?.ToString()))
        {
            Debug.WriteLine("ReturnCommandParameter is null or empty.");
            return;
        }

        XElement? el = Vector3Data.XmlElement?.Element("POSITION")?.Element(textField.ReturnCommandParameter.ToString());
        if (el == null)
        {
            Debug.WriteLine($"Element for {textField.ReturnCommandParameter} not found.");
            return;
        }

        Debug.WriteLine($"Adding field link for {Vector3Data.XmlElement?.BaseUri}.{textField.ReturnCommandParameter}.");

        XMLReaderSystem.AddElementLink(new TextFieldData() { Field = textField }, el);
    }
}