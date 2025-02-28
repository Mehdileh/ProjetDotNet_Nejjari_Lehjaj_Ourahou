using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Gauniv.Client.Models;

namespace Gauniv.Client.Services
{
    public class GameService : IDisposable
    {
        private readonly HttpClient _httpClient;

        public GameService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5231/api/") // 🔥 Adapter si nécessaire
            };
        }

        /// ✅ **Récupérer la liste des jeux**
        public async Task<List<Game>> GetGamesAsync()
        {
            Console.WriteLine("📌 Appel de GetGamesAsync()...");

            try
            {
                var response = await _httpClient.GetAsync("games");
                string json = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📌 JSON reçu : {json}"); // 🔥 Ajout du log pour voir la réponse API

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Erreur API : {json}");
                    return new List<Game>();
                }

                var games = JsonSerializer.Deserialize<List<Game>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                }) ?? new List<Game>();

                Console.WriteLine($"✅ {games.Count} jeux récupérés !");
                return games;
            }
            catch (JsonException jex)
            {
                Console.WriteLine($"❌ Erreur JSON : {jex.Message}");
                return new List<Game>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetGamesAsync : {ex.Message}");
                return new List<Game>();
            }
        }



        /// ✅ **Récupérer les jeux possédés**
        public async Task<List<Game>> GetOwnedGamesAsync(string token)
        {
            try
            {
                SetAuthorizationHeader(token);
                var response = await _httpClient.GetAsync("games/owned");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Erreur API : {await response.Content.ReadAsStringAsync()}");
                    return new List<Game>();
                }

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Game>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Game>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur GetOwnedGamesAsync : {ex.Message}");
                return new List<Game>();
            }
        }

        /// 🔥 **Ajout du token dans l'Authorization Header**
        private void SetAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
