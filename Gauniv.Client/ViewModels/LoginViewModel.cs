using System;
using System.Windows.Input;
using Gauniv.Client.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Microsoft.Maui.Storage;

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
            Debug.WriteLine("🔍 Tentative de connexion...");

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
                return;
            }

            Debug.WriteLine($"📡 Envoi des identifiants : Email={Email}, Password=******");

            // 🔥 Récupérer le token JWT et le rôle
            var (token, role) = await _authService.LoginAsync(Email, Password);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(role))
            {
                Debug.WriteLine("❌ Authentification échouée !");
                await Application.Current.MainPage.DisplayAlert("Erreur", "Email ou mot de passe incorrect", "OK");
                return;
            }

            // ✅ Stocker le token et le rôle
            Preferences.Set("token", token);
            Preferences.Set("role", role);

            Debug.WriteLine($"✅ Token enregistré : {Preferences.Get("token", "Aucun token")}");
            Debug.WriteLine($"✅ Rôle enregistré : {Preferences.Get("role", "Aucun rôle")}");
            Debug.WriteLine($"✅ Connexion réussie en tant que : {role}");

            // 🔥 Redirection selon le rôle
            if (role == "Admin")
            {
                Debug.WriteLine("🚀 Redirection vers GameAdminPage");
                await Shell.Current.GoToAsync("GameAdminPage");
            }
            else
            {
                Debug.WriteLine("🚀 Redirection vers GamePage");
                await Shell.Current.GoToAsync("GamePage");
            }
        }
    }
}
