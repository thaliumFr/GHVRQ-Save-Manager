using GHVRQ_Save_Manager.Data;
using GHVRQ_Save_Manager.XML;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;
using UraniumUI.Material.Controls;

namespace GHVRQ_Save_Manager.Components;

public partial class Vector3InputComponent : ContentView
{
	public Vector3InputComponent()
	{
		InitializeComponent();
    }

    public static readonly BindableProperty XElementValueProperty = BindableProperty.Create(
        nameof(XElementValue),
        typeof(XElement),
        typeof(Vector3InputComponent),
        null);

    public XElement XElementValue
    {
        get => (XElement)GetValue(XElementValueProperty);
        set => SetValue(XElementValueProperty, value);
    }

    public static readonly BindableProperty Vector3valProperty = BindableProperty.Create(
        nameof(Vector3val),
        typeof(Vector3),
        typeof(Vector3InputComponent),
        Vector3.Zero);

    public Vector3 Vector3val
    {
        get => (Vector3)GetValue(Vector3valProperty);
        set => SetValue(Vector3valProperty, value);
    }

    private void AddFieldToList(object sender, EventArgs e)
    {
        TextField textField = (TextField)sender;
        if (String.IsNullOrEmpty(textField.ReturnCommandParameter?.ToString()))
        {
            Debug.WriteLine("ReturnCommandParameter is null or empty.");
            return;

        }

        XElement? el = XElementValue?.Element("POSITION")?.Element(textField.ReturnCommandParameter.ToString());
        if (el == null)
        {
            Debug.WriteLine($"Element for {textField.ReturnCommandParameter} not found.");
            return;
        }
        XMLReaderSystem.AddElementLink(new TextFieldData() { Field = textField }, el);
    }
}