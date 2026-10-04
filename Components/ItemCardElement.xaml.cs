using ABI.System.Numerics;
using GHVRQ_Save_Manager.Data;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Vector3 = System.Numerics.Vector3;

namespace GHVRQ_Save_Manager.Components;

public partial class ItemCardComponent : ContentView
{
	public ItemCardComponent()
	{
        InitializeComponent();
    }

    public static readonly BindableProperty ItemProperty = BindableProperty.Create(
        nameof(Item),
        typeof(GHVRObject),
        typeof(ItemCardComponent),
        new GHVRObject());

    public GHVRObject Item
    {
        get => (GHVRObject)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    private void BackpackButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Debug.WriteLine($"Adding item to backpack: {button.CommandParameter}");
        
    }
}