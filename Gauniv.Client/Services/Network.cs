using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Maui.Storage;
using System.Diagnostics; // Permet d'utiliser SecureStorage

namespace Gauniv.Client.Services
{
    internal partial class NetworkService : ObservableObject
    {
        public static NetworkService Instance { get; private set; } = new NetworkService();

        [ObservableProperty]
        private string token;

        public HttpClient HttpClient { get; private set; }

        public event Action OnConnected;
        public event Action OnDisconnected;

        private NetworkService()
        {
            HttpClient = new HttpClient();

            // 🔹 Vérifier si un token est déjà stocké au démarrage
            LoadToken();
        }

        /// <summary>
        /// 🔹 Charge le token depuis SecureStorage au démarrage
        /// </summary>
        public void LoadToken()
        {
            Token = SecureStorage.GetAsync("auth_token").Result;
            if (!string.IsNullOrEmpty(Token))
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                Debug.WriteLine($"🔹 Token chargé au démarrage : {Token}");
                OnConnected?.Invoke();
            }
        }

        /// <summary>
        /// 🔹 Méthode pour enregistrer le token, le stocker et notifier la connexion
        /// </summary>
        public void Connect(string newToken)
        {
            Token = newToken;

            // 🔹 Stocker le token de manière persistante
            SecureStorage.SetAsync("auth_token", Token);

            // 🔹 Ajouter automatiquement le token à chaque requête HTTP
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            Debug.WriteLine($"✅ Connexion réussie - Token enregistré et ajouté aux requêtes.");

            OnConnected?.Invoke(); // 🔥 Notifie que l'utilisateur est connecté
        }

        /// <summary>
        /// 🔹 Méthode pour supprimer le token et notifier la déconnexion
        /// </summary>
        public void Disconnect()
        {
            Token = null;

            // 🔹 Supprimer le token du stockage sécurisé
            SecureStorage.Remove("auth_token");

            // 🔹 Supprimer l'authentification des requêtes HTTP
            HttpClient.DefaultRequestHeaders.Authorization = null;
            Debug.WriteLine("🚪 Déconnexion - Token supprimé.");

            OnDisconnected?.Invoke(); // 🔥 Notifie que l'utilisateur est déconnecté
        }
    }
}
