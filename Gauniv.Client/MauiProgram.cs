using CommunityToolkit.Maui;
using Gauniv.Client.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;

namespace Gauniv.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // 🔹 Enregistrer NetworkService en singleton
            builder.Services.AddSingleton<NetworkService>();

            var app = builder.Build();

            Task.Run(() =>
            {
                // 🔥 Initialisation réseau / connexion serveur (si nécessaire)
                NetworkService.Instance.LoadToken();
            });

            return app;
        }
    }
}
