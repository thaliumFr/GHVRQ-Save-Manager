#if ANDROID26_0_OR_GREATER
using Android.Content;
using Android.Provider;
#endif
using GHVRQ_Save_Manager.Systems;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace GHVRQ_Save_Manager
{
    public partial class App : Application
    {
        public App()
        {

            InitializeComponent();
            Tools.CheckForAdb();

        }



        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window win = new(new AppShell())
            {
                Width = 1280,
                Height = 720,
                Title = "GHVRQ Save Manager"
            };

            
            return win;
        }
    }
}