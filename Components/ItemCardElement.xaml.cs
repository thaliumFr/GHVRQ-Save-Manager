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

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if(propertyName == "Item")
        {
            Vector3Data data = new()
            {
                Vector3 = Item.position,
                XmlElement = Item.XMLElement
                
            };

            Debug.WriteLine($"ItemCardComponent OnPropertyChanged: {data.Vector3.ToString()}, {data.XmlElement?.Name ?? "null"}");

            ItemPositionComponent.Vector3Data = data;
        }
    }



    public static readonly BindableProperty XMLElementProperty = BindableProperty.Create(
    nameof(XMLElement),
    typeof(XElement),
    typeof(ItemCardComponent),
    null);

    public XElement XMLElement
    {
        get => (XElement)GetValue(XMLElementProperty);
        set => SetValue(XMLElementProperty, value);
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
    
    public static readonly BindableProperty ItemPositionProperty = BindableProperty.Create(
        nameof(ItemPosition),
        typeof(Vector3),
        typeof(ItemCardComponent),
        Vector3.Zero);


    public Vector3 ItemPosition
    {
        get => (Vector3)GetValue(ItemPositionProperty);
        set => SetValue(ItemPositionProperty, value);
    }

    public static readonly BindableProperty ItemProperty = BindableProperty.Create(
        nameof(Item),
        typeof(GHVRObject),
        typeof(ItemCardComponent),
        new GHVRObject());

    private void SetItemValue(GHVRObject value)
    {
        SetValue(ItemProperty, value);
        ItemPosition = value.position;
        ItemID = value.type;
        ItemUUID = value.Object_Id;
        XMLElement = value.XMLElement;

        Debug.WriteLine($"ItemCardComponent SetItemValue: {ItemPosition.ToString()}, {XMLElement?.ToString() ?? "null"}");
    }

    public GHVRObject Item
    {
        get => (GHVRObject)GetValue(ItemProperty);
        set => SetItemValue(value);
    }

    private void BackpackButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Debug.WriteLine($"Adding item to backpack: {button.CommandParameter}");
        
    }
}