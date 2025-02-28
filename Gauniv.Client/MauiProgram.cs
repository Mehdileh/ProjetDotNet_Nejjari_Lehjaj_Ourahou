using CommunityToolkit.Maui;
using Gauniv.Client.Services;
using Gauniv.Client.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.DependencyInjection;

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

            // 🔹 Enregistrement des services et ViewModels
            builder.Services.AddHttpClient<GameService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5231/api/"); // 🔥 Adapter si nécessaire
            });

            builder.Services.AddSingleton<GameListViewModel>();

            // 🔹 Enregistrer NetworkService si nécessaire
            builder.Services.AddSingleton<NetworkService>();

            var app = builder.Build();

            // Plus besoin de `Task.Run(() => NetworkService.Instance.LoadToken());` 
            // car `NetworkService` est enregistré en Singleton

            return app;
        }
    }
}
