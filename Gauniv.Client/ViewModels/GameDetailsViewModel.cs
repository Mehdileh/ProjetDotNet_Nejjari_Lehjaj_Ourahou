using System.Threading.Tasks;
using Gauniv.Client.Models;
using Gauniv.Client.Services;

namespace Gauniv.Client.ViewModels
{
    public class GameDetailsViewModel : BaseViewModel
    {
        private readonly GameService _gameService;
        private Game _game;

        public Game Game
        {
            get => _game;
            set
            {
                _game = value;
                OnPropertyChanged();
            }
        }

        // ✅ Ajout d'un constructeur sans paramètre pour le XAML
        public GameDetailsViewModel()
        {
            _gameService = new GameService();
        }

        // ✅ Ajout d'une méthode pour charger le jeu après instanciation
        public async Task Initialize(int gameId)
        {
            Game = await _gameService.GetGameByIdAsync(gameId);
        }
    }
}
