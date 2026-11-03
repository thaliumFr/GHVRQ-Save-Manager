using GHVRQ_Save_Manager.Data;
using GHVRQ_Save_Manager.XML;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;
using UraniumUI.Material.Controls;

namespace GHVRQ_Save_Manager.Components;

public partial class Vector4InputComponent : ContentView
{
	public Vector4InputComponent()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty XElementValueProperty = BindableProperty.Create(
        nameof(XElementValue),
        typeof(XElement),
        typeof(Vector4InputComponent),
        null);

    public XElement XElementValue
    {
        get => (XElement)GetValue(XElementValueProperty);
        set => SetValue(XElementValueProperty, value);
    }

    public static readonly BindableProperty Vector4valProperty = BindableProperty.Create(
        nameof(Vector4val),
        typeof(Vector4),
        typeof(Vector4InputComponent),
        Vector4.Zero);

    public Vector4 Vector4val
    {
        get => (Vector4)GetValue(Vector4valProperty);
        set => SetValue(Vector4valProperty, value);
    }

    private void AddFieldToList(object sender, EventArgs e)
    {
        IUniversalInputFieldData.AddToList<TextField, TextFieldData>(sender, XElementValue.Element("ROTATION")!);
    }
}