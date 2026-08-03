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
            OnQuestDetected += (_, status) =>
            {
                IsQuestDetected = status;

                if (IsQuestDetected)
                {
                    SaveFileLoader.ExtractSaveFiles(out Result[] results);
                }
                else
                {
                    SaveFileLoader.ClearSaveFilesFolder();
                }
            };
            App.recurringTask.StartAsync(new());

            SaveFileLoader.LoadSaveFiles();
            //foreach (var file in SaveFileLoader.Files)
            //{
            //    FilesList.Add(new SaveFileListItem() { Path = file.FilePath });
            //}

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
    }
}
