using GHVRQ_Save_Manager.Resources.Components;
using GHVRQ_Save_Manager.Systems;

namespace GHVRQ_Save_Manager
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            SaveFileLoader.LoadSaveFiles();
            foreach (var file in SaveFileLoader.Files)
            {
                FilesList.Add(new SaveFileListItem() { Path = file.FilePath });
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            SaveFileLoader.ExtractSaveFiles(out Result[] results);

            string errors = results.Aggregate("", (acc, result) => acc + result.Error);

            if (!string.IsNullOrEmpty(errors))
            {
                DisplayAlert("Error", "An error occurred while extracting save files.", "OK");
            }
        }
    }
}
