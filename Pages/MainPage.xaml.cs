using GHVRQ_Save_Manager.Components;
using GHVRQ_Save_Manager.Data;
using GHVRQ_Save_Manager.Systems;
using GHVRQ_Save_Manager.XML;
using System.Diagnostics;
using UraniumUI.Material.Controls;
using UraniumUI.Pages;

namespace GHVRQ_Save_Manager
{
    public partial class MainPage : UraniumContentPage
    {
        public static event EventHandler<bool>? OnQuestDetected;
        public static bool IsQuestDetected;

        public static Dictionary<TextField, string> TextFieldsList = [];

        private string SaveFilePath = "";

        public MainPage()
        {
            TextFieldsList = [];
            InitializeComponent();

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
                List<GHVRObject> objects = XMLReaderSystem.GetAllGHVRObjects();

                ItemsFlexLayoutList.Children.Clear();
                foreach (GHVRObject obj in objects)
                {
                    ItemsFlexLayoutList.Children.Add(
                        new ItemCardComponent()
                        {
                            ItemID = obj.type,
                            ItemUUID = obj.Object_Id
                        }
                    );
                }
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


            OutputLabel.Text = "Checking for devices...";
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

        public void AddFieldToList(object sender, EventArgs e)
        {
            TextField textField = (TextField)sender;
            if(!TextFieldsList.ContainsKey(textField))
            {
                TextFieldsList.Add(textField, textField.Text);
                textField.Text = "";
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

        private void ItemSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
