using GHVRQ_Save_Manager.Data;
using UraniumUI.Controls;
using UraniumUI.Material.Controls;

namespace GHVRQ_Save_Manager.Components;

public partial class ConsumableComponent : ContentView
{
    public Data.ConsumableComponent? Component =>
    BindingContext as Data.ConsumableComponent?;

    public ConsumableComponent()
	{
        InitializeComponent();
        StateDropdown.ItemsSource = Enum.GetNames<Data.ConsumableComponent.ConsumableState>();
	}

    private void TextField_Loaded(object sender, EventArgs e)
    {
        TextField TextField = (TextField)sender;
        IUniversalInputFieldData.AddToList(sender, Component?.XMLElement?.Element(TextField.ReturnCommandParameter.ToString())!);
    }

    private void StateDropdown_Loaded(object sender, EventArgs e)
    {
        Dropdown dropdown = (Dropdown)sender;
        IUniversalInputFieldData.AddToList(sender, Component?.XMLElement?.Element(dropdown.CommandParameter.ToString())!);
    }
}