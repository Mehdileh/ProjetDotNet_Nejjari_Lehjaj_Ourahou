using Gauniv.Client.ViewModel;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.Pages
{
    public partial class GamePage : ContentPage
    {
        private readonly GameViewModel _viewModel;

        public GamePage()
        {
            InitializeComponent();
            _viewModel = new GameViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Console.WriteLine("📌 `OnAppearing()` appelé, chargement des jeux...");
            await _viewModel.LoadGamesCommand.ExecuteAsync(null);
        }
    }
}
