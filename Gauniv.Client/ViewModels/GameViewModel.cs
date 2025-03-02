using System.Collections.Generic;
using System.Linq;
using Gauniv.Client.Models;

namespace Gauniv.Client.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        public Game Game { get; set; }

        public GameViewModel(Game game)
        {
            Game = game;
        }

        public string CategoriesString => string.Join(", ", Game.Categories);




    }
}
