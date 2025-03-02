using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Gauniv.Client.Models;
using Gauniv.Client.Services;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace Gauniv.Client.ViewModels
{
    public class GameAdminViewModel : BaseViewModel
    {
        private readonly GameService _gameService;
        public ObservableCollection<Game> Games { get; set; } = new();
        public ObservableCollection<string> Categories { get; set; } = new();

        // Champs pour ajouter/modifier un jeu
        public string GameName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string SelectedCategory { get; set; }

        private Game _selectedGame;
        public Game SelectedGame
        {
            get => _selectedGame;
            set
            {
                _selectedGame = value;
                OnPropertyChanged(nameof(SelectedGame));

                if (_selectedGame != null)
                {
                    GameName = _selectedGame.Name;
                    Description = _selectedGame.Description;
                    Price = _selectedGame.Price;
                    SelectedCategory = string.Join(", ", _selectedGame.Categories); // ✅ Correction ici
                    OnPropertyChanged(nameof(GameName));
                    OnPropertyChanged(nameof(Description));
                    OnPropertyChanged(nameof(Price));
                    OnPropertyChanged(nameof(SelectedCategory));
                }
            }
        }

        public ICommand AddGameCommand { get; }
        public ICommand UpdateGameCommand { get; }
        public ICommand DeleteGameCommand { get; }
        public ICommand LoadGamesCommand { get; }

        public GameAdminViewModel()
        {
            _gameService = new GameService();

            AddGameCommand = new Command(async () => await AddGame());
            UpdateGameCommand = new Command(async () => await UpdateGame());
            DeleteGameCommand = new Command(async () => await DeleteGame());
            LoadGamesCommand = new Command(async () => await LoadGames());

            _ = LoadGames();
        }

        private async Task LoadGames()
        {
            var games = await _gameService.GetGamesAsync();
            Games = new ObservableCollection<Game>(games);
            OnPropertyChanged(nameof(Games));
        }


        private async Task AddGame()
        {
            bool success = await _gameService.AddGameAsync(GameName, Description, Price, SelectedCategory);
            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Succès", "Jeu ajouté avec succès", "OK");
                await LoadGames();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Échec de l'ajout du jeu", "OK");
            }
        }

        private async Task UpdateGame()
        {
            if (SelectedGame == null) return;

            SelectedGame.Name = GameName;
            SelectedGame.Description = Description;
            SelectedGame.Price = Price;
            SelectedGame.Categories = new List<string> { SelectedCategory }; // ✅ Correction ici

            bool success = await _gameService.UpdateGameAsync(SelectedGame);
            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Succès", "Jeu mis à jour", "OK");
                await LoadGames();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Échec de la mise à jour", "OK");
            }
        }

        private async Task DeleteGame()
        {
            if (SelectedGame == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmation", "Voulez-vous supprimer ce jeu ?", "Oui", "Non");
            if (!confirm) return;

            bool success = await _gameService.DeleteGameAsync(SelectedGame.Id);
            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Succès", "Jeu supprimé", "OK");
                await LoadGames();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Échec de la suppression", "OK");
            }
        }
    }
}