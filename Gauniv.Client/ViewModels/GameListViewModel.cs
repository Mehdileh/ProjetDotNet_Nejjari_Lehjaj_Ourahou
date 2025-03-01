using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Gauniv.Client.Models;
using Gauniv.Client.Services;
using System.Linq;

namespace Gauniv.Client.ViewModels
{
    public class GameListViewModel : BaseViewModel
    {
        private readonly GameService _gameService;
        public ObservableCollection<Game> Games { get; set; } = new();

        private bool _showOwnedGames;
        public bool ShowOwnedGames
        {
            get => _showOwnedGames;
            set
            {
                if (_showOwnedGames != value)
                {
                    _showOwnedGames = value;
                    OnPropertyChanged(nameof(ShowOwnedGames));
                    _ = LoadGames(_showOwnedGames);
                }
            }
        }

        private Game? _selectedGame;
        public Game? SelectedGame
        {
            get => _selectedGame;
            set
            {
                _selectedGame = value;
                OnPropertyChanged(nameof(SelectedGame));
            }
        }

        public ICommand BuyGameCommand { get; }
        public ICommand PlayGameCommand { get; }

        public GameListViewModel()
        {
            _gameService = new GameService();
            BuyGameCommand = new Command<Game>(async (game) => await BuyGame(game));
            PlayGameCommand = new Command<Game>(async (game) => await PlayGame(game));

            _ = LoadGames();
        }

        public async Task LoadGames(bool showOwnedGames = false)
        {
            var ownedGames = await _gameService.GetOwnedGamesAsync();
            var allGames = await _gameService.GetGamesAsync();

            foreach (var game in allGames)
            {
                game.IsOwned = ownedGames.Any(g => g.Id == game.Id);
            }

            var filteredGames = showOwnedGames ? allGames.Where(g => g.IsOwned).ToList() : allGames;

            Games = new ObservableCollection<Game>(filteredGames);
            OnPropertyChanged(nameof(Games));
        }

        private async Task BuyGame(Game game)
        {
            if (game == null) return;

            bool success = await _gameService.BuyGameAsync(game.Id);
            if (success)
            {
                await Shell.Current.DisplayAlert("Achat réussi", "Le jeu a été ajouté à votre bibliothèque.", "OK");
                await LoadGames(ShowOwnedGames);
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Impossible d'acheter ce jeu.", "OK");
            }
        }

        private async Task PlayGame(Game game)
        {
            if (game == null) return;

            await Shell.Current.DisplayAlert("Lancement du jeu", $"Démarrage de {game.Name}...", "OK");
        }
    }
}
