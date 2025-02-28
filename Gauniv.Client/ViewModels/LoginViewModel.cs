using System;
using System.Windows.Input;
using Gauniv.Client.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new Command(async () => await Login());
            NavigateToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));
        }

        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
                return;
            }

            var token = await _authService.LoginAsync(Email, Password);
            if (string.IsNullOrEmpty(token))
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Email ou mot de passe incorrect", "OK");
                return;
            }

            // ✅ Stocker le token et rediriger vers la page des jeux
            Preferences.Set("AuthToken", token);
            await Shell.Current.GoToAsync("//games");
        }
    }
}
