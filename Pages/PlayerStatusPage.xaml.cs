using UraniumUI.Material.Controls;

namespace GHVRQ_Save_Manager.Pages;

public partial class PlayerStatusPage : ContentView
{
	public PlayerStatusPage()
	{
		InitializeComponent();
	}

    public static void AddFieldToList(object sender, EventArgs e)
    {
        MainPage.AddFieldToList(sender, e);
    }
}