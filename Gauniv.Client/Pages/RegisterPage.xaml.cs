using Gauniv.Client.ViewModels;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private readonly RegisterViewModel _viewModel;

        public RegisterPage()
        {
            InitializeComponent();
            _viewModel = new RegisterViewModel();
            BindingContext = _viewModel;
        }
    }
}
