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

        }
    }
}
