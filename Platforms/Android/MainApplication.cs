using Android.App;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using GHVRQ_Save_Manager.Systems;

namespace GHVRQ_Save_Manager
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {

        }

        public static async Task CheckAndRequestPermissions()
        {

            PermissionStatus statusRead = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            PermissionStatus statusWrite = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            Permissions.ShouldShowRationale<Permissions.StorageRead>();
            Permissions.ShouldShowRationale<Permissions.StorageWrite>();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
