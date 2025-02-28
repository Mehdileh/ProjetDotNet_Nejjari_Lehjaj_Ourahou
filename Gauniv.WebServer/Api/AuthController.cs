using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Gauniv.WebServer.Api
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration; // Injection pour accéder à la clé secrète

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        /// ✅ **Génération du Token JWT avec Rôles**
        private async Task<string> GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // ✅ Récupération de la clé depuis la configuration
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            // 🔹 **Récupérer les rôles de l'utilisateur**
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

            // 🔹 **Définir les claims JWT**
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }.Union(roleClaims); // Ajouter les rôles

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// 📌 **POST /api/auth/register** - Crée un nouvel utilisateur
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,  // Ajout du prénom
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Utilisateur créé avec succès !" });
        }

        /// 📌 **POST /api/auth/login** - Authentifie un utilisateur et génère un JWT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Email ou mot de passe incorrect.");

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);
            if (!result.Succeeded)
                return Unauthorized("Email ou mot de passe incorrect.");

            // ✅ Générer un token avec les rôles
            var token = await GenerateJwtToken(user);

            return Ok(new
            {
                message = "Connexion réussie !",
                token = token  // Ajout du token JWT avec les rôles
            });
        }

        /// 📌 **POST /api/auth/assign-role** - Assigne un rôle à un utilisateur
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("Utilisateur non trouvé.");

            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest("Ce rôle n'existe pas.");

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return BadRequest($"L'utilisateur {user.Email} a déjà le rôle {model.Role}.");

            await _userManager.AddToRoleAsync(user, model.Role);
            return Ok($"✅ Rôle {model.Role} assigné à {user.Email}");
        }
    }
}
