using Gauniv.Client.ViewModels;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;

        public LoginPage()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            BindingContext = _viewModel;
        }
    }
}
