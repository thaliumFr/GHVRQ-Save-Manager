using GHVRQ_Save_Manager.Data;
using GHVRQ_Save_Manager.Systems;
using GHVRQ_Save_Manager.XML;
using MvvmHelpers;
using System.Diagnostics;
using UraniumUI.Material.Controls;
using UraniumUI.Pages;
using CheckBox = UraniumUI.Material.Controls.CheckBox;

namespace GHVRQ_Save_Manager
{
    public partial class MainPage : UraniumContentPage
    {
        public static event EventHandler<bool>? OnQuestDetected;
        public static bool IsQuestDetected;

        public static Dictionary<IUniversalFieldData, string> InputFieldPathDict = [];
        List<GHVRObject> objects = [];
        List<GHVRObject> filteredObjects = [];

        public ObservableRangeCollection<GHVRObject> ObjectsToDisplay { get; set; }

        private string SaveFilePath = "";

        int HeaderHeight = 200;
        int SearchBarheight = 60;

        public MainPage()
        {
            InputFieldPathDict = [];
            ObjectsToDisplay = [];
            InitializeComponent();

            // Setting up event handlers

            ADB.OnDevicesChanged += ADB_OnDevicesChanged;

            SaveFileLoader.OnSaveFileLoaded+= (_, files) => {
                Debug.WriteLine($"Save file loaded: {files.Count}");
                SaveFileDropdown.ItemsSource = files.Select(file => $"{file.FileName} ({file.Type})").ToList();
            };

            SaveFileLoader.OnSaveFilesExtracted += (_, results) =>
            {
                SaveFileLoader.LoadSaveFiles();
            };

            XMLReaderSystem.OnXMLDocumentLoaded += (_, _) =>
            {
                Debug.WriteLine("XML Document loaded");
                objects = XMLReaderSystem.GetAllGHVRObjects();
                filteredObjects = objects;
                UpdateObjectsToDisplay();
            };


            OnQuestDetected += (_, status) =>
            {
                IsQuestDetected = status;

                if (IsQuestDetected)
                {
                    Debug.WriteLine("Quest Detected, extracting save files");
                    SaveFileLoader.ExtractSaveFiles(out Result[] results);
                }
                else
                {
                    SaveFileLoader.ClearSaveFilesFolder();
                }
            };

            App.recurringTask.StartAsync(new());

            TabbedView.SelectedTabChanged += (_, e) =>
            {
                Debug.WriteLine($"Current tab: {e.Title}");
                Debug.WriteLine($"Items count: {filteredObjects.Count}/{objects.Count}");
            };

            //Setting Tab content

            OutputLabel.Text = "Checking for devices...";

            this.BindingContext = this;
            ItemsCollectionView.SetBinding(ItemsView.ItemsSourceProperty, static (MainPage p) => p.ObjectsToDisplay);

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(100);

           SizeChanged += (_, e) =>
            {
                if (Height > HeaderHeight)
                {
                    double newHeight = Height - HeaderHeight;
                    Debug.WriteLine($"Setting height to {newHeight}, Height is {Height}");
                    PlayerStatusScrollView.MaximumHeightRequest = newHeight;

                    ItemScrollView.MaximumHeightRequest = newHeight - SearchBarheight;
                    MapScrollView.MaximumHeightRequest = newHeight;
                }
            };
        }

        private void ADB_OnDevicesChanged(object? sender, ADB.DeviceData[] devices)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (devices != null && devices.Length > 0)
                {
                    if (devices[0].Model.Contains("Quest"))
                    {
                        OnQuestDetected?.Invoke(null, true);
                    }
                    OutputLabel.Text = $"Device ID: {devices[0].ID}\nModel: {devices[0].Model}";
                }
                else
                {
                    OnQuestDetected?.Invoke(null, false);
                    OutputLabel.Text = "No device connected.";
                    SaveFileDropdown.IsEnabled = false;
                    SaveFileDropdown.ItemsSource = new List<string>();
                }
            });
        }

        private void SaveFileDropdown_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine($"Selected save file: {SaveFileDropdown.SelectedItem}");
            TabbedView.IsVisible = !String.IsNullOrEmpty(SaveFileDropdown?.SelectedItem?.ToString());


            if (XMLReaderSystem.CurrentDoc != null)
            {
                XMLReaderSystem.CurrentDoc.Save(SaveFilePath);
                Debug.WriteLine("XML Document saved");
            }

            if (TabbedView.IsVisible)
            {
                SaveFileLoader.NameToPath(SaveFileDropdown.SelectedItem.ToString(), out string fileName, out string filePath);
                SaveFilePath = filePath;
                XMLReaderSystem.Load(filePath);
            }
        }

        public static void AddFieldToList(object sender, EventArgs e)
        {
            object field = sender;

            if (field is CheckBox checkBox)
            {
                ToggleFieldData toggleFieldData = new ()
                {
                    Field = checkBox
                };

                if (!InputFieldPathDict.ContainsKey(toggleFieldData) && !String.IsNullOrEmpty($"{checkBox.CommandParameter}"))
                {
                    InputFieldPathDict.Add(toggleFieldData, $"{checkBox.CommandParameter}");
                }
            }
            else if (field is TextField textField)
            {
                TextFieldData textFieldData = new ()
                {
                    Field = textField
                };

                if (!InputFieldPathDict.ContainsKey(textFieldData) && !String.IsNullOrEmpty($"{textField.ReturnCommandParameter}"))
                {
                    InputFieldPathDict.Add(textFieldData, $"{textField.ReturnCommandParameter}");
                }
            }
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (XMLReaderSystem.CurrentDoc != null)
            {
                XMLReaderSystem.CurrentDoc.Save(SaveFilePath);
                Debug.WriteLine("XML Document saved");
            }
            SaveFileLoader.PushSaveFiles(out var results);
        }

        private void UpdateObjectsToDisplay()
        {
            ObjectsToDisplay.Clear();
            ObjectsToDisplay.AddRange(filteredObjects);
        }


        private void ItemSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBar searchBar = (SearchBar)sender;
            if (String.IsNullOrEmpty(searchBar.Text)) filteredObjects = objects;
            else if( int.TryParse(searchBar.Text, out int result))
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
}
