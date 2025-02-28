using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Gauniv.Client.Models;
using Gauniv.Client.Services;
using Gauniv.Client.ViewModels;

namespace Gauniv.Client.ViewModels
{
    public class GameListViewModel : BaseViewModel
    {
        private readonly GameService _gameService;
        public ObservableCollection<Game> Games { get; set; } = new();

        public GameListViewModel()
        {
            _gameService = new GameService();
            LoadGames();
        }

        public async void LoadGames()
        {
            var games = await _gameService.GetGamesAsync();
            Games.Clear();
            foreach (var game in games)
            {
                Games.Add(game);
            }
        }
    }
}
