using Gauniv.Client.Models;
using Gauniv.Client.ViewModels;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.Pages
{
    public partial class GamePage : ContentPage
    {
        public GamePage()
        {
            InitializeComponent();
        }

        private async void OnGameSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Game selectedGame)
            {
                await Navigation.PushAsync(new GameDetailsPage(selectedGame.Id));
            }
        }
    }
}
