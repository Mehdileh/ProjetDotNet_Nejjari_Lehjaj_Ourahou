using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Gauniv.Client.Models;

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

        public async Task<List<Game>> GetGamesAsync()
        {
            var response = await _httpClient.GetAsync("games");
            if (!response.IsSuccessStatusCode) return new List<Game>();

            string json = await response.Content.ReadAsStringAsync();
            var gamesList = JsonSerializer.Deserialize<GameList>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return gamesList?.Games ?? new List<Game>();
        }

        public async Task<Game> GetGameByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"games/{id}");
            if (!response.IsSuccessStatusCode) return null;

            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Game>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }

    class GameList
    {
        public int TotalCount { get; set; }
        public List<Game> Games { get; set; } = new();
    }
}
