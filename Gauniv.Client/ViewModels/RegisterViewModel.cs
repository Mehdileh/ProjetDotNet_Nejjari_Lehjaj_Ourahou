using System;
using System.Windows.Input;
using Gauniv.Client.Services;
using Gauniv.Client.Models; // ✅ Utilisation du bon RegisterModel
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public RegisterViewModel()
        {
            _authService = new AuthService();
            RegisterCommand = new Command(async () => await Register());
            NavigateToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));
        }

        private async Task Register()
        {
            var success = await _authService.RegisterAsync(new RegisterModel
            {
                Username = Username,
                Email = Email,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName
            });

            if (!success)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Échec de l'inscription", "OK");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Succès", "Compte créé avec succès !", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
