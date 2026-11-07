using GHVRQ_Save_Manager.Data;
using GHVRQ_Save_Manager.XML;
using System.Diagnostics;
using UraniumUI.Controls;
using UraniumUI.Material.Controls;

namespace GHVRQ_Save_Manager.Components;

public partial class DamagableCardComponent : ContentView
{
    public DamagableComponent? Component =>
        BindingContext as DamagableComponent?;

    public DamagableCardComponent()
	{
        BindingContext = this;
        InitializeComponent();
	}

    private void TextField_Loaded(object sender, EventArgs e)
    {
        TextField TextField = (TextField)sender;
        IUniversalInputFieldData.AddToList(sender, Component?.XMLElement?.Element(TextField.ReturnCommandParameter.ToString())!);
    }
}