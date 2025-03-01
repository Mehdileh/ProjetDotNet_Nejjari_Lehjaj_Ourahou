using System;
using System.Windows.Input;
using Gauniv.Client.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Microsoft.Maui.Storage; // 🔥 Ajouté pour Preferences

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
                Debug.WriteLine("⚠️ Champs vides !");
                await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
                return;
            }

            Debug.WriteLine($"📡 Envoi des identifiants : Email={Email}, Password=******");

            var token = await _authService.LoginAsync(Email, Password);

            Debug.WriteLine($"📩 Token reçu : {token ?? "Aucun token"}");

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("❌ Authentification échouée !");
                await Application.Current.MainPage.DisplayAlert("Erreur", "Email ou mot de passe incorrect", "OK");
                return;
            }

            // ✅ Stocker le token et rediriger vers la page des jeux
            Preferences.Set("token", token);
            Debug.WriteLine($"✅ Token enregistré : {Preferences.Get("token", "Aucun token")}");

            // 🔥 Ajout : Affichage temporaire pour vérifier si le token est bien stocké
            await Application.Current.MainPage.DisplayAlert("Connexion réussie", $"Token : {token}", "OK");

            await Shell.Current.GoToAsync("//games");
        }
    }
}
