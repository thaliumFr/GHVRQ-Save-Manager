using GHVRQ_Save_Manager.Data;
using GHVRQ_Save_Manager.Pages;
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

        public static Dictionary<IUniversalInputFieldData, string> InputFieldPathDict = [];


        private string SaveFilePath = "";

        static int HeaderHeight = 200;
        static int SearchBarheight = 60;

        public MainPage()
        {
            InputFieldPathDict = [];
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

                ItemsPage.UpdateGHVRObjectsList();
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
            };

            //Setting Tab content

            if (Tools.IsDebug)
            {
                SaveFileLoader.LoadSaveFiles();
                SaveFileDropdown.IsEnabled = true;
            }

            OutputLabel.Text = "Checking for devices...";

            this.BindingContext = this;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(100);

           SizeChanged += (_, e) =>
            {
                if (Height > HeaderHeight)
                {
                    PlayerStatusScrollView.MaximumHeightRequest = GetDesiredElementHeight(Height, false);
                    ItemsPageView.MaximumHeightRequest = GetDesiredElementHeight(Height, true);
                    MapScrollView.MaximumHeightRequest = GetDesiredElementHeight(Height, false);
                }
            };
        }

        public static double GetDesiredElementHeight(double height,bool withSearchBar = false)
        {
            double newHeight = height - HeaderHeight;
            if (withSearchBar)
            {
                return newHeight - SearchBarheight;
            }
            else
            {
                return newHeight;
            }
            
        }

        private void ADB_OnDevicesChanged(object? sender, ADB.DeviceData[] devices)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Tools.IsDebug){
                    OutputLabel.Text = "Debug mode, skipping device detection.";
                    return;
                }
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

    }
}
