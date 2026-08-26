using GHVRQ_Save_Manager.Data;
using GHVRQ_Save_Manager.Systems;
using GHVRQ_Save_Manager.XML;
using System.Diagnostics;
using System.Xml.Linq;
using UraniumUI.Material.Controls;
using UraniumUI.Pages;
using UraniumUI.ViewExtensions;

namespace GHVRQ_Save_Manager
{
    public partial class MainPage : UraniumContentPage
    {
        public static event EventHandler<bool>? OnQuestDetected;
        public static bool IsQuestDetected;

        public static Dictionary<TextField, string> TextFieldsList = [];

        public SaveFileBindableData? SaveFile;

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
                }
            });
        }

        private void SaveFileDropdown_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine($"Selected save file: {SaveFileDropdown.SelectedItem}");
            TabbedView.IsVisible = !String.IsNullOrEmpty(SaveFileDropdown?.SelectedItem?.ToString());

            if (TabbedView.IsVisible)
            {
                SaveFileLoader.NameToPath(SaveFileDropdown.SelectedItem.ToString(), out string fileName, out string filePath);
                XMLReaderSystem.Load(filePath);
            }
        }

        public void AddFieldToList(object sender, EventArgs e)
        {
            TextField textField = (TextField)sender;
            TextFieldsList.Add(textField, textField.Text);
            textField.Text = "";
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            SaveFileLoader.PushSaveFiles(out var results);
        }
    }
}
