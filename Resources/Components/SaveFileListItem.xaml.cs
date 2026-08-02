using GHVRQ_Save_Manager.Systems;

namespace GHVRQ_Save_Manager.Resources.Components;

public partial class SaveFileListItem : ContentView
{
    public SaveFileListItem()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty PathProperty = BindableProperty.Create(
		nameof(Path),
		typeof(string),
		typeof(SaveFileListItem),
		string.Empty);

    public string Path
	{
		get => (string) GetValue(PathProperty);
		set => SetValue(PathProperty, value);
    }

    private void Load_Button_Clicked(object sender, EventArgs e)
    {

    }
}