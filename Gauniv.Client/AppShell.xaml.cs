using Gauniv.Client.Pages;
using Microsoft.Maui.Controls;

namespace Gauniv.Client
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(GameDetailsPage), typeof(GameDetailsPage));
            Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
            Routing.RegisterRoute(nameof(GameAdminPage), typeof(GameAdminPage));

        }
    }
}
