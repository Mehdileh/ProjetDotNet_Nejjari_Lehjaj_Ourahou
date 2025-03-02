using Gauniv.WebServer.Data;
using Gauniv.WebServer.Websocket;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Gauniv.WebServer.Services
{
    public class SetupService : IHostedService
    {
        private ApplicationDbContext? applicationDbContext;
        private readonly IServiceProvider serviceProvider;

        public SetupService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                if (applicationDbContext is null)
                {
                    throw new Exception("ApplicationDbContext is null");
                }

                if (applicationDbContext.Database.GetPendingMigrations().Any())
                {
                    applicationDbContext.Database.Migrate();
                }

                // 🔥 Création des rôles "Admin" et "Joueur"
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = { "Admin", "Joueur" };

                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                        Console.WriteLine($"✅ Rôle créé : {roleName}");
                    }
                }

                // 🔥 Création des utilisateurs par défaut
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                await CreateDefaultUser(userManager, "d7d3f090-51e3-458c-aa63-b28422274d44", "admin@example.com", "admin123", "Admin", "Nizar", "Nejjari", "Admin");
                await CreateDefaultUser(userManager, "cc160613-0f12-4347-bca4-85aa28f93b86", "player@example.com", "player123", "Player", "John", "Doe", "Joueur");

                // 🔥 Ajout des catégories par défaut
                var defaultCategories = new List<Category>
                {
                    new Category { Name = "RPG" }, new Category { Name = "Open World" },
                    new Category { Name = "Racing" }, new Category { Name = "Shooter" },
                    new Category { Name = "Football" }, new Category { Name = "Survival" },
                    new Category { Name = "MOBA" }, new Category { Name = "Hack & Slash" },
                    new Category { Name = "Battle Royale" }, new Category { Name = "FPS" },
                    new Category { Name = "Survival Horror" }, new Category { Name = "MMORPG" },
                    new Category { Name = "Action-Adventure" }
                };

                foreach (var category in defaultCategories)
                {
                    if (!await applicationDbContext.Categories.AnyAsync(c => c.Name == category.Name))
                    {
                        applicationDbContext.Categories.Add(category);
                        Console.WriteLine($"✅ Catégorie ajoutée : {category.Name}");
                    }
                }

                await applicationDbContext.SaveChangesAsync();


                // 🔍 Récupérer les catégories depuis la base
                var rpgCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "RPG");
                var openWorldCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Open World");
                var racingCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Racing");
                var shooterCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Shooter");
                var footballCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Football");
                var survivalCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Survival");
                var mobaCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "MOBA");
                var hackAndSlashCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Hack & Slash");
                var battleRoyaleCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Battle Royale");
                var fpsCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "FPS");
                var survivalHorrorCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Survival Horror");
                var mmorpgCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "MMORPG");
                var actionAdventureCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Action-Adventure");

                if (rpgCategory == null || openWorldCategory == null || racingCategory == null ||
                    shooterCategory == null || footballCategory == null || survivalCategory == null ||
                    mobaCategory == null || hackAndSlashCategory == null || battleRoyaleCategory == null ||
                    fpsCategory == null || survivalHorrorCategory == null || mmorpgCategory == null ||
                    actionAdventureCategory == null)
                {
                    Console.WriteLine("🚨 Erreur : Une ou plusieurs catégories n'ont pas été trouvées !");
                    return;
                }

                // 🔥 Ajout des jeux par défaut
                var categories = applicationDbContext.Categories.ToList();
                var defaultGames = new List<Game>
{
    new Game { Name = "The Witcher 3", Description = "RPG Open World", Price = 39.99M, FilePath = "C:\\games\\witcher3.exe", Categories = GetCategories(categories, "RPG", "Open World") },
    new Game { Name = "Grand Theft Auto 6", Description = "RPG Open World", Price = 70.99M, FilePath = "C:\\games\\GTA6.exe", Categories = GetCategories(categories, "RPG", "Open World") },
    new Game { Name = "Cyberpunk 2077", Description = "Futuristic RPG", Price = 49.99M, FilePath = "C:\\games\\cyberpunk2077.exe", Categories = GetCategories(categories, "RPG", "Open World") },
    new Game { Name = "Diablo IV", Description = "Hack & Slash RPG", Price = 59.99M, FilePath = "C:\\games\\Diablo4.exe", Categories = GetCategories(categories, "Hack & Slash", "RPG") },
    new Game { Name = "Red Dead Redemption 2", Description = "Western Open World", Price = 50.00M, FilePath = "C:\\games\\RDR2.exe", Categories = GetCategories(categories, "Open World") },
    new Game { Name = "Forza Horizon 5", Description = "Racing Game", Price = 45.00M, FilePath = "C:\\games\\FH5.exe", Categories = GetCategories(categories, "Racing") },
    new Game { Name = "Halo Infinite", Description = "FPS Sci-Fi", Price = 39.99M, FilePath = "C:\\games\\HaloInfinite.exe", Categories = GetCategories(categories, "Shooter", "FPS") },
    new Game { Name = "League of Legends", Description = "MOBA Game", Price = 0.00M, FilePath = "C:\\games\\LoL.exe", Categories = GetCategories(categories, "MOBA") },
    new Game { Name = "Fortnite", Description = "Battle Royale", Price = 0.00M, FilePath = "C:\\games\\Fortnite.exe", Categories = GetCategories(categories, "Battle Royale") },
    new Game { Name = "Call of Duty: MW3", Description = "Modern FPS", Price = 69.99M, FilePath = "C:\\games\\CODMW3.exe", Categories = GetCategories(categories, "Shooter", "FPS") },
    new Game { Name = "Resident Evil 4", Description = "Survival Horror", Price = 40.00M, FilePath = "C:\\games\\RE4.exe", Categories = GetCategories(categories, "Survival Horror") },
    new Game { Name = "World of Warcraft", Description = "MMORPG Classic", Price = 14.99M, FilePath = "C:\\games\\WoW.exe", Categories = GetCategories(categories, "MMORPG") },
    new Game { Name = "Minecraft", Description = "Survival Sandbox", Price = 29.99M, FilePath = "C:\\games\\Minecraft.exe", Categories = GetCategories(categories, "Survival") },
    new Game { Name = "FIFA 24", Description = "Football Simulation", Price = 59.99M, FilePath = "C:\\games\\FIFA24.exe", Categories = GetCategories(categories, "Football") },
    new Game { Name = "The Legend of Zelda: Breath of the Wild", Description = "Action-Adventure", Price = 50.00M, FilePath = "C:\\games\\ZeldaBOTW.exe", Categories = GetCategories(categories, "Action-Adventure") }
};

                foreach (var game in defaultGames)
                {
                    if (!await applicationDbContext.Games.AnyAsync(g => g.Name == game.Name))
                    {
                        applicationDbContext.Games.Add(game);
                        Console.WriteLine($"✅ Jeu ajouté : {game.Name}");
                    }
                }

                await applicationDbContext.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        // ✅ Création utilisateur corrigée
        private async Task CreateDefaultUser(UserManager<User> userManager, string userId, string email, string password, string username, string firstName, string lastName, string role)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                user = new User
                {
                    Id = userId,
                    UserName = username,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    Console.WriteLine($"✅ {role} créé : {email} (ID: {userId})");

                    // 🎮 Si c'est le joueur par défaut, on lui attribue des jeux
                    if (userId == "cc160613-0f12-4347-bca4-85aa28f93b86")
                    {
                        await AssignDefaultGamesToPlayer(user);
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Erreur lors de la création de {email} : {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"✅ Utilisateur {email} déjà existant (ID: {userId})");
            }
        }

        // ✅ Assignation de jeux pour player@example.com
        private async Task AssignDefaultGamesToPlayer(User user)
        {
            if (applicationDbContext == null)
            {
                Console.WriteLine("❌ Erreur : applicationDbContext est null !");
                return;
            }

            var defaultGames = await applicationDbContext.Games
                                .Where(g => g.Name == "The Witcher 3" ||
                                            g.Name == "Grand Theft Auto 6" ||
                                            g.Name == "Minecraft")
                                .ToListAsync();

            if (defaultGames.Any())
            {
                foreach (var game in defaultGames)
                {
                    var userGame = new UserGame
                    {
                        UserId = user.Id,
                        GameId = game.Id
                    };

                    applicationDbContext.UserGames.Add(userGame);
                    Console.WriteLine($"🎮 {game.Name} attribué à {user.Email}");
                }

                await applicationDbContext.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("⚠️ Aucun jeu par défaut trouvé dans la base de données.");
            }
        }

        // ✅ Optimisation pour récupérer les catégories
        private List<Category> GetCategories(List<Category> categories, params string[] categoryNames)
        {
            return categories.Where(c => categoryNames.Contains(c.Name)).ToList();
        }
    }
}
