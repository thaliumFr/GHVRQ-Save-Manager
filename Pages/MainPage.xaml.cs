using GHVRQ_Save_Manager.Resources.Components;
using GHVRQ_Save_Manager.Systems;
using System.Diagnostics;
using UraniumUI.Pages;
using static ADB;

namespace GHVRQ_Save_Manager
{
    public partial class MainPage : UraniumContentPage
    {
        public static event EventHandler<bool> OnQuestDetected;
        public static bool IsQuestDetected;

        public MainPage()
        {
            InitializeComponent();

            ADB.OnDevicesChanged += ADB_OnDevicesChanged;

            SaveFileLoader.OnSaveFileLoaded+= (_, files) => {
                Debug.WriteLine($"Save file loaded: {files.Count}");
                SaveFileDropdown.ItemsSource = files.Select(file => $"{file.FileName} ({file.Type})").ToList();
            };

            SaveFileLoader.OnSaveFileExtracted += (_, results) =>
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
        }
    }
}
