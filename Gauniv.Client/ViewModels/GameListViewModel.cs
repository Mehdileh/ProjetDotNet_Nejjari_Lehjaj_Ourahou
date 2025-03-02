using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Gauniv.Client.Models;
using Gauniv.Client.Services;
using Microsoft.Maui.Storage;

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

        private string _searchName;
        public string SearchName
        {
            get => _searchName;
            set
            {
                if (_searchName != value)
                {
                    _searchName = value;
                    OnPropertyChanged(nameof(SearchName));
                    _ = LoadGames();
                }
            }
        }

        private decimal? _minPrice;
        public decimal? MinPrice
        {
            get => _minPrice;
            set
            {
                if (_minPrice != value)
                {
                    _minPrice = value;
                    OnPropertyChanged(nameof(MinPrice));
                    _ = LoadGames();
                }
            }
        }

        private decimal? _maxPrice;
        public decimal? MaxPrice
        {
            get => _maxPrice;
            set
            {
                if (_maxPrice != value)
                {
                    _maxPrice = value;
                    OnPropertyChanged(nameof(MaxPrice));
                    _ = LoadGames();
                }
            }
        }

        private int? _selectedCategory;
        public int? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                    _ = LoadGames();
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
        public ICommand UninstallGameCommand { get; }

        public ICommand ApplyFiltersCommand { get; }


        public GameListViewModel()
        {
            _gameService = new GameService();
            BuyGameCommand = new Command<Game>(async (game) => await BuyGame(game));
            UninstallGameCommand = new Command<Game>(async (game) => await UninstallGame(game));
            ApplyFiltersCommand = new Command(async () => await LoadGames());

            _ = LoadGames();
        }

        public async Task LoadGames(bool showOwnedGames = false)
        {
            var ownedGames = await _gameService.GetOwnedGamesAsync();
            var allGames = await _gameService.GetGamesAsync(
                name: SearchName,
                minPrice: MinPrice,
                maxPrice: MaxPrice
            );

            foreach (var game in allGames)
            {
                game.IsOwned = ownedGames.Any(g => g.Id == game.Id);
            }

            var filteredGames = showOwnedGames ? allGames.Where(g => g.IsOwned).ToList() : allGames;

            Games = new ObservableCollection<Game>(filteredGames);
            OnPropertyChanged(nameof(Games));
        }

        public async Task CheckIfOwned(Game game)
        {
            var isOwned = await _gameService.CheckGameOwnershipAsync(game.Id);
            game.IsOwned = isOwned;
        }



        private async Task BuyGame(Game game)
        {
            if (game == null) return;

            bool success = await _gameService.BuyGameAsync(game.Id);
            if (success)
            {
                game.IsOwned = true;
                await Shell.Current.DisplayAlert("Achat réussi", "Le jeu a été ajouté à votre bibliothèque.", "OK");
                await LoadGames(ShowOwnedGames);
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Impossible d'acheter ce jeu.", "OK");
            }
        }

        private async Task UninstallGame(Game game)
        {
            if (game == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmation",
                "Voulez-vous vraiment désinstaller ce jeu ?", "Oui", "Non");

            if (!confirm) return;

            bool success = await _gameService.UninstallGameAsync(game.Id);
            if (success)
            {
                await Shell.Current.DisplayAlert("Désinstallation réussie", "Le jeu a été retiré de votre bibliothèque.", "OK");
                await LoadGames(ShowOwnedGames);
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Impossible de désinstaller ce jeu.", "OK");
            }
        }

    }
}
