using Gauniv.Client.Models;
using Gauniv.Client.ViewModels;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Gauniv.Client.Pages
{
    public partial class GamePage : ContentPage
    {
        private readonly GameListViewModel _viewModel;

        public GamePage()
        {
            InitializeComponent();
            _viewModel = new GameListViewModel();
            BindingContext = _viewModel;
        }

        // Filtrer les jeux possédés
        private async void OnFilterChanged(object sender, CheckedChangedEventArgs e)
        {
            await _viewModel.LoadGames(e.Value);
        }

        // Naviguer vers la page de détails du jeu sélectionné
        private async void OnGameSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Game selectedGame)
            {
                await Navigation.PushAsync(new GameDetailsPage(selectedGame.Id));
            }
        }
    }
}
