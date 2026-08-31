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

    public static readonly BindableProperty ItemUIDProperty = BindableProperty.Create(
        nameof(TheItemIdFR),
        typeof(string),
        typeof(ItemCardComponent),
        string.Empty);

    public string TheItemIdFR
    {
        get => (string)GetValue(ItemUIDProperty);
        set => SetValue(ItemUIDProperty, value);
    }
}