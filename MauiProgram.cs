using GHVRQ_Save_Manager.WinUI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using UraniumUI;

namespace GHVRQ_Save_Manager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("LiberationSans.ttf", "LiberationSans");
                    fonts.AddFontAwesomeIconFonts();
                });

            builder.Services.AddHostedService<ADBRecurringTask>();
            

#if DEBUG
            builder.Logging.AddDebug();
#endif

            
            return builder.Build();
        }
    }
}
