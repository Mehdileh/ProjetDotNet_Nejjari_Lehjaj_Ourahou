using Gauniv.Client.ViewModels;

namespace Gauniv.Client.Pages
{
    public partial class GameDetailsPage : ContentPage
    {
        private readonly GameDetailsViewModel _viewModel;

        public GameDetailsPage(int gameId)
        {
            InitializeComponent();
            _viewModel = new GameDetailsViewModel();
            BindingContext = _viewModel;
            _viewModel.Initialize(gameId); // ✅ Appelle Initialize après la création
        }
    }
}
