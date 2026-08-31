using System.Diagnostics;
using System.Reflection.Metadata;
using System.Windows.Input;

namespace GHVRQ_Save_Manager.Components;

public partial class ItemCardComponent : ContentView
{
	public ItemCardComponent()
	{
        InitializeComponent();
    }

    public static readonly BindableProperty ItemIDProperty = BindableProperty.Create(
        nameof(ItemID),
        typeof(EItemID),
        typeof(ItemCardComponent),
        EItemID.NONE);

    public EItemID ItemID
    {
        get => (EItemID)GetValue(ItemIDProperty);
        set => SetValue(ItemIDProperty, value);
    }

    public static readonly BindableProperty ItemUUIDProperty = BindableProperty.Create(
        nameof(ItemUUID),
        typeof(string),
        typeof(ItemCardComponent),
        string.Empty);

    public string ItemUUID
    {
        get => (string)GetValue(ItemUUIDProperty);
        set => SetValue(ItemUUIDProperty, value);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("clicked");
        Button button = (Button)sender;
        Debug.WriteLine($"Adding item to backpack: {button.CommandParameter}");
        
    }
}