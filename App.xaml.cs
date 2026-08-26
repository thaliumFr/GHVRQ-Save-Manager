using GHVRQ_Save_Manager.Systems;
using GHVRQ_Save_Manager.XML;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace GHVRQ_Save_Manager
{
    public partial class App : Application
    {
        public static ADBRecurringTask recurringTask = new(logger: IPlatformApplication.Current?.Services?.GetService<ILogger<ADBRecurringTask>>());
        public App()
        {
            InitializeComponent();
        }

        

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window win = new(new AppShell())
            {
                Width = 1280,
                Height = 720,
                Title = "GHVRQ Save Manager"
            };

            win.Destroying += (s, e) => {
                XMLReaderSystem.CurrentDoc = null;
                SaveFileLoader.ClearSaveFilesFolder();
            };

            
            return win;
        }
    }
}