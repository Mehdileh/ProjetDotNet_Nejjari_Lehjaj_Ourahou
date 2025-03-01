using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Gauniv.Client.Models;
using Microsoft.Maui.Storage; // Pour Preferences

namespace Gauniv.Client.Services
{
    public class GameService
    {
        private readonly HttpClient _httpClient;

        public GameService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5231/api/")
            };
        }

        // ✅ Récupérer tous les jeux disponibles
        public async Task<List<Game>> GetGamesAsync()
        {
            try
            {
                Debug.WriteLine("📡 Envoi de la requête GET /games...");
                var response = await _httpClient.GetAsync("games");

                Debug.WriteLine($"📩 Réponse reçue : {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("❌ Erreur lors de la récupération des jeux !");
                    return new List<Game>();
                }

                string json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"📜 JSON reçu : {json}");

                var gamesList = JsonSerializer.Deserialize<GameList>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Debug.WriteLine($"✅ {gamesList?.Games.Count} jeux chargés !");
                return gamesList?.Games ?? new List<Game>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 Exception GetGamesAsync : {ex.Message}");
                return new List<Game>();
            }
        }

        // ✅ Récupérer un jeu spécifique par son ID
        public async Task<Game> GetGameByIdAsync(int id)
        {
            try
            {
                Debug.WriteLine($"📡 Envoi de la requête GET /games/{id}...");
                var response = await _httpClient.GetAsync($"games/{id}");

                Debug.WriteLine($"📩 Réponse reçue : {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("❌ Erreur lors de la récupération du jeu !");
                    return null;
                }

                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Game>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 Exception GetGameByIdAsync : {ex.Message}");
                return null;
            }
        }

        // ✅ Récupérer les jeux possédés par l'utilisateur
        public async Task<List<Game>> GetOwnedGamesAsync()
        {
            try
            {
                var token = Preferences.Get("token", string.Empty);
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("🚫 Aucun token trouvé, utilisateur non connecté !");
                    return new List<Game>();
                }

                // 🔥 DEBUG: Vérification du token avant requête
                Debug.WriteLine($"🔑 Token récupéré depuis Preferences : {token}");

                // ⚠️ IMPORTANT: Supprimer les anciens headers pour éviter les erreurs
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // 🔥 Ajout correct du header Authorization
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Debug.WriteLine($"📡 Envoi de la requête GET /games/owned avec token : {token.Substring(0, 10)}...");

                // 🔥 Envoi de la requête via HttpRequestMessage
                using var request = new HttpRequestMessage(HttpMethod.Get, "games/owned");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);

                Debug.WriteLine($"📩 Réponse reçue : {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"❌ Erreur API GET /games/owned : {response.StatusCode}");
                    Debug.WriteLine($"🔎 Headers envoyés : {string.Join(", ", request.Headers)}");
                    return new List<Game>();
                }

                string json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"📜 Réponse JSON : {json}");

                return JsonSerializer.Deserialize<List<Game>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 Exception GetOwnedGamesAsync : {ex.Message}");
                return new List<Game>();
            }
        }



        // ✅ Acheter un jeu
        public async Task<bool> BuyGameAsync(int gameId)
        {
            try
            {
                string token = Preferences.Get("token", string.Empty);
                Debug.WriteLine($"🔑 Récupération du token : {token}");

                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("🚫 Aucun token trouvé, impossible d'acheter !");
                    return false;
                }

                // ⚠️ Réinitialiser et ajouter l’en-tête Authorization correctement
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Debug.WriteLine($"📡 Envoi de la requête POST /games/{gameId}/buy...");
                var response = await _httpClient.PostAsync($"games/{gameId}/buy", null);

                Debug.WriteLine($"📩 Réponse reçue : {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("❌ Erreur lors de l'achat du jeu !");
                    return false;
                }

                Debug.WriteLine("✅ Achat réussi !");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 Exception BuyGameAsync : {ex.Message}");
                return false;
            }
        }
    }

    // ✅ Modèle pour la liste des jeux
    class GameList
    {
        public int TotalCount { get; set; }
        public List<Game> Games { get; set; } = new();
    }
}
