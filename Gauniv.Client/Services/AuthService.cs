using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
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

        public async Task<(string Token, string Role)> LoginAsync(string email, string password)
        {
            var loginData = new { Email = email, Password = password };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("login", content);
            if (!response.IsSuccessStatusCode)
            {
                return (null, null); // Erreur de connexion
            }

            var result = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<LoginResponse>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (jsonResponse == null || string.IsNullOrEmpty(jsonResponse.Token))
                return (null, null);

            // 🔥 Extraire le rôle du token JWT
            var role = ExtractRoleFromToken(jsonResponse.Token);
            return (jsonResponse.Token, role);
        }

        private string ExtractRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // 🔥 Afficher tous les claims pour vérifier la structure du token
            foreach (var claim in jwtToken.Claims)
            {
                Debug.WriteLine($"🔹 Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            // 🔥 Récupération du rôle sous différentes clés possibles
            var roleClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == "role" ||
                c.Type == "roles" ||
                c.Type == ClaimTypes.Role);

            return roleClaim?.Value ?? "User"; // Par défaut : User
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
        public string Role { get; set; }
    }
}
