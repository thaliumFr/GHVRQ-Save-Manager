using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using GHVRQ_Save_Manager.Systems;

namespace GHVRQ_Save_Manager
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : MauiAppCompatActivity
    {
        public MainActivity(): base()
        {

            

        }
    }
}
