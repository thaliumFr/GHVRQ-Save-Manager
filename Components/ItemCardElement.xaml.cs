using GHVRQ_Save_Manager.Data;
using System.Diagnostics;
using UraniumUI.Controls;

namespace GHVRQ_Save_Manager.Components;

public partial class ItemCardComponent : ContentView
{
    public ItemCardComponent()
    {
        BindingContext = this;
        InitializeComponent();

        ItemCatDropdown.ItemsSource = Enum.GetNames<BackpackObject.Category>();

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

        if(Item.IsInBackpack)
        {
            Debug.WriteLine($"Removing item from backpack: {button.CommandParameter}");
            Item.RemoveFromBackpack();
        }
        else
        {
            Debug.WriteLine($"Adding item to backpack: {button.CommandParameter}");
            Item.AddToBackpack(BackpackObject.Category.BACKPACK_TOOLS, -1, -1);
        }
        Debug.WriteLine($"Item: {Item.Position}");
    }

    private void PageTextField_TextChanged(object sender, TextChangedEventArgs e)
    {
        Item.BackpackObject?.XMLElement?.Element("PAGE")?.Value = PageTextField.Text;
    }

    private void SlotTextField_TextChanged(object sender, TextChangedEventArgs e)
    {
        Item.BackpackObject?.XMLElement?.Element("SLOT")?.Value = SlotTextField.Text;
    }

    private void ItemCatDropdown_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "SelectedItem")
        {
            var selectedCategory = Enum.Parse<BackpackObject.Category>(ItemCatDropdown?.SelectedItem?.ToString());
            Debug.WriteLine($"Selected Category: {selectedCategory}");
            if (Item.BackpackObject != null)
            {
                Item.BackpackObject.category = selectedCategory;
                Item.BackpackObject.XMLElement?.Element("CATEGORY")?.SetValue(selectedCategory.ToString());
            }
        }
    }
}