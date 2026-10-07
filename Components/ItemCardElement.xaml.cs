using GHVRQ_Save_Manager.Data;
using System.Diagnostics;

namespace GHVRQ_Save_Manager.Components;

public partial class ItemCardComponent : ContentView
{
	public ItemCardComponent()
    {
        BindingContext = this;
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
    private void UpdateInBackpackButton()
    {
        if (Item.IsInBackpack)
        {
            BackpackButton.Text = "Remove from Backpack";
            BackpackButton.BackgroundColor = Colors.Red;
        }
        else
        {
            BackpackButton.Text = "Add to Backpack";
            BackpackButton.BackgroundColor = Colors.Green;
        }
        BackpackGrid.IsVisible = Item.IsInBackpack;
    }



    private void BackpackButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;

        if(Item.IsInBackpack)
        {
            Debug.WriteLine($"Removing item from backpack: {button.CommandParameter}");
            Item.RemoveFromBackpack();
        }
        else
        {
            Debug.WriteLine($"Adding item to backpack: {button.CommandParameter}");
            Item.AddToBackpack(BackpackObject.Category.BACKPACK_TOOLS, 9, 9);
        }
        Debug.WriteLine($"Item: {Item.Position}");

        UpdateInBackpackButton();
    }


    private void ItemCard_Loaded(object sender, EventArgs e)
    {
        UpdateInBackpackButton();
    }
}