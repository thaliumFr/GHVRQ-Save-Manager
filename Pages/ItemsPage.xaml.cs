using GHVRQ_Save_Manager.Data;
using GHVRQ_Save_Manager.XML;
using MvvmHelpers;
using System.Diagnostics;

namespace GHVRQ_Save_Manager.Pages;

public partial class ItemsPage : ContentView
{
    static List<GHVRObject> objects = [];
    static List<GHVRObject> filteredObjects = [];

    public ObservableRangeCollection<GHVRObject> ObjectsToDisplay { get; set; }

    static event EventHandler OnUpdateObjectsCollection;

    public ItemsPage()
	{
        ObjectsToDisplay = [];
		InitializeComponent();

        OnUpdateObjectsCollection += (_, e) =>
        {
            UpdateObjectsToDisplay();
        };

        SizeChanged += (w, e) =>
        {
            ItemsCollectionView.HeightRequest = Height;
        };
    }

    public static void UpdateGHVRObjectsList()
    {
        objects = XMLReaderSystem.GetAllGHVRObjects();
        filteredObjects = objects;

        OnUpdateObjectsCollection?.Invoke(null, new());
    }


    private void UpdateObjectsToDisplay()
    {
        ObjectsToDisplay.Clear();
        ObjectsToDisplay.AddRange(filteredObjects);

        ItemsCollectionView.ItemsSource = ObjectsToDisplay;

        Debug.WriteLine($"found {ObjectsToDisplay.Count} objects");
    }

    private void ItemSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        if (String.IsNullOrEmpty(searchBar.Text)) filteredObjects = objects;
        else if (int.TryParse(searchBar.Text, out int result))
        {
            filteredObjects = objects.Where(o => o.Object_Id == searchBar.Text).ToList();
        }
        else
        {
            filteredObjects = objects.Where(o => o.type.ToString().Contains(searchBar.Text.ToUpper())).ToList();
        }
        Debug.WriteLine($"Found {filteredObjects.Count} with filter");
        UpdateObjectsToDisplay();
    }

    private void ItemSearchBar_Search(object sender, EventArgs e)
    {
        ItemSearchBar_TextChanged(sender, new TextChangedEventArgs(ItemSearchBar.Text, ItemSearchBar.Text));
    }
}