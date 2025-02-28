using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Models;
using Gauniv.Client.Services;

namespace Gauniv.Client.ViewModel
{
    public partial class GameViewModel : ObservableObject
    {
        private readonly GameService _gameService;

        [ObservableProperty]
        private ObservableCollection<Game> games = new();

        [ObservableProperty]
        private ObservableCollection<Game> ownedGames = new();

        public GameViewModel()
        {
            _gameService = new GameService();
            LoadGamesCommand = new AsyncRelayCommand(LoadGames);
            LoadOwnedGamesCommand = new AsyncRelayCommand<string>(LoadOwnedGames);
        }

        public AsyncRelayCommand LoadGamesCommand { get; }
        private async Task LoadGames()
        {
            Console.WriteLine("📌 Chargement des jeux...");

            var gameList = await _gameService.GetGamesAsync();
            if (gameList.Count == 0)
            {
                Console.WriteLine("❌ Aucun jeu récupéré !");
            }
            else
            {
                Console.WriteLine($"✅ {gameList.Count} jeux trouvés !");
            }

            Games.Clear();
            foreach (var game in gameList)
            {
                Games.Add(game);
            }
        }

        public AsyncRelayCommand<string> LoadOwnedGamesCommand { get; }
        private async Task LoadOwnedGames(string token)
        {
            Console.WriteLine("📌 Chargement des jeux possédés...");
            var ownedList = await _gameService.GetOwnedGamesAsync(token);

            OwnedGames.Clear();
            foreach (var game in ownedList)
            {
                OwnedGames.Add(game);
            }
        }
    }
}
