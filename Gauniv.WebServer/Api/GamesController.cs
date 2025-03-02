using AutoMapper;
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Gauniv.WebServer.Api
{

    [Route("api/games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public GamesController(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// 📌 **GET /api/games** - Liste des jeux avec filtres et pagination
        [HttpGet]
        public async Task<IActionResult> GetGames(
            [FromQuery] string? name,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int? categoryId,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var query = _context.Games
                .Include(g => g.Categories)
                .AsQueryable();

            // 🔍 Filtre par nom (si fourni)
            if (!string.IsNullOrEmpty(name))
                query = query.Where(g => g.Name.Contains(name));

            // 🔍 Filtre par prix minimum (si fourni)
            if (minPrice.HasValue)
                query = query.Where(g => g.Price >= minPrice.Value);

            // 🔍 Filtre par prix maximum (si fourni)
            if (maxPrice.HasValue)
                query = query.Where(g => g.Price <= maxPrice.Value);

            // 🔍 Filtre par catégorie (si fournie)
            if (categoryId.HasValue)
                query = query.Where(g => g.Categories.Any(c => c.Id == categoryId.Value));

            // 📊 Nombre total de jeux après filtrage
            var totalCount = await query.CountAsync();

            // 📌 Application de la pagination
            var games = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            // 🎯 Conversion en DTO
            var gamesDto = _mapper.Map<List<GameDto>>(games);

            return Ok(new
            {
                totalCount,
                games = gamesDto
            });
        }


        /// 📌 **POST /api/games** - Ajouter un jeu avec ses catégories
        [HttpPost]
        public async Task<IActionResult> AddGame([FromBody] GameDto gameDto)
        {
            if (gameDto == null || string.IsNullOrWhiteSpace(gameDto.Name))
                return BadRequest("Les données du jeu sont invalides.");

            // Vérifier si les catégories existent dans la base
            var categories = await _context.Categories
                .Where(c => gameDto.Categories.Contains(c.Name))
                .ToListAsync();

            // Mapper GameDto vers Game
            var game = _mapper.Map<Game>(gameDto);

            // Associer les catégories trouvées
            game.Categories = categories;

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            // Retourner la réponse avec les catégories associées
            var gameDtoResponse = _mapper.Map<GameDto>(game);
            return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, gameDtoResponse);
        }


        /// 📌 **GET /api/games/{id}** - Obtenir un jeu par ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameById(int id)
        {
            var game = await _context.Games
                .Include(g => g.Categories)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound($"Le jeu avec l'ID {id} n'existe pas.");

            var gameDto = _mapper.Map<GameDto>(game);
            return Ok(gameDto);
        }

        /// 📌 **DELETE /api/games/{id}** - Supprimer un jeu
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound($"Le jeu avec l'ID {id} n'existe pas.");

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// 📌 **GET /api/games/{id}/download** - Télécharger un jeu
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound($"Le jeu avec l'ID {id} n'existe pas.");

            if (!System.IO.File.Exists(game.FilePath))
                return NotFound("Le fichier du jeu est introuvable.");

            var memory = new MemoryStream();
            using (var stream = new FileStream(game.FilePath, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", Path.GetFileName(game.FilePath));
        }

        /// 📌 **POST /api/games/{id}/categories/{categoryId}** - Associer une catégorie à un jeu
        [HttpPost("{id}/categories/{categoryId}")]
        public async Task<IActionResult> AddCategoryToGame(int id, int categoryId)
        {
            var game = await _context.Games.Include(g => g.Categories).FirstOrDefaultAsync(g => g.Id == id);
            var category = await _context.Categories.FindAsync(categoryId);

            if (game == null || category == null)
                return NotFound("Le jeu ou la catégorie n'existe pas.");

            if (game.Categories.Contains(category))
                return BadRequest("Le jeu possède déjà cette catégorie.");

            game.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<GameDto>(game));
        }

        /// 📌 **DELETE /api/games/{id}/categories/{categoryId}** - Retirer une catégorie d'un jeu
        [HttpDelete("{id}/categories/{categoryId}")]
        public async Task<IActionResult> RemoveCategoryFromGame(int id, int categoryId)
        {
            var game = await _context.Games.Include(g => g.Categories).FirstOrDefaultAsync(g => g.Id == id);
            var category = await _context.Categories.FindAsync(categoryId);

            if (game == null || category == null)
                return NotFound("Le jeu ou la catégorie n'existe pas.");

            if (!game.Categories.Contains(category))
                return BadRequest("Le jeu ne possède pas cette catégorie.");

            game.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// 📌 **POST /api/games/{id}/buy** - Acheter un jeu
        [HttpPost("{id}/buy")]
        [Authorize] // Nécessite que l'utilisateur soit connecté
        public async Task<IActionResult> BuyGame(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Utilisateur non authentifié.");

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("Utilisateur non trouvé.");

            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound($"Le jeu avec l'ID {id} n'existe pas.");

            // Vérifier si l'utilisateur possède déjà ce jeu
            var existingEntry = await _context.UserGames
                .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GameId == id);

            if (existingEntry != null)
                return BadRequest("Vous possédez déjà ce jeu.");

            // Ajouter une entrée dans la table UserGames
            var newUserGame = new UserGame { UserId = userId, GameId = id };
            _context.UserGames.Add(newUserGame);
            await _context.SaveChangesAsync();

            return Ok($"✅ Jeu '{game.Name}' acheté avec succès !");
        }

        /// 📌 **GET /api/games/owned** - Liste des jeux achetés par l'utilisateur
        [HttpGet("owned")]
        [Authorize]
        public async Task<IActionResult> GetOwnedGames()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            Console.WriteLine($"📡 Header Authorization reçu : {authHeader}");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                Console.WriteLine("❌ Aucun utilisateur authentifié !");
                return Unauthorized("Utilisateur non authentifié.");
            }

            Console.WriteLine($"✅ Utilisateur authentifié : {userId}");

            var ownedGames = await _context.UserGames
                .Where(ug => ug.UserId == userId)
                .Select(ug => ug.Game)
                .ToListAsync();

            var ownedGamesDto = _mapper.Map<List<GameDto>>(ownedGames);
            return Ok(ownedGamesDto);
        }

        /// 📌 **GET /api/games/{id}/owned** - Vérifier si un jeu est acheté par l'utilisateur
        [HttpGet("owned/{id}")]
        [Authorize]
        public async Task<IActionResult> IsGameOwned(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Utilisateur non authentifié.");

            var isOwned = await _context.UserGames
                .AnyAsync(ug => ug.UserId == userId && ug.GameId == id);

            return Ok(new { owned = isOwned });
        }

        /// 📌 **DELETE /api/games/{id}/uninstall** - Désinstaller un jeu (le retirer de la bibliothèque de l'utilisateur)
        [HttpDelete("{id}/uninstall")]
        [Authorize] // Nécessite que l'utilisateur soit connecté
        public async Task<IActionResult> UninstallGame(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Utilisateur non authentifié.");

            var userGame = await _context.UserGames
                .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GameId == id);

            if (userGame == null)
                return NotFound("Vous ne possédez pas ce jeu.");

            _context.UserGames.Remove(userGame);
            await _context.SaveChangesAsync();

            return Ok($"✅ Jeu désinstallé avec succès !");
        }







    }
}
