using Gauniv.Client.Models;
using Gauniv.Client.Services;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Gauniv.Client.Pages
{
    public partial class MyGamesPage : ContentPage
    {
        private readonly GameService _gameService;
        public ObservableCollection<Game> OwnedGames { get; set; } = new();

        public MyGamesPage()
        {
            InitializeComponent();
            _gameService = new GameService();
            GamesListView.ItemsSource = OwnedGames;
            LoadOwnedGames();
        }

        private async void LoadOwnedGames()
        {
            var games = await _gameService.GetOwnedGamesAsync();
            OwnedGames.Clear(); // Nettoyer avant d'ajouter
            foreach (var game in games)
            {
                OwnedGames.Add(game);
            }
        }

    }
}
