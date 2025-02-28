using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gauniv.Client.Models;

namespace Gauniv.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5231/api/auth/") };
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var loginData = new { Email = email, Password = password };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("login", content);
            if (!response.IsSuccessStatusCode)
            {
                return null; // Erreur de connexion
            }

            var result = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<LoginResponse>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return jsonResponse?.Token; // Retourne le token JWT
        }

        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("register", content);
            return response.IsSuccessStatusCode;
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }
}
