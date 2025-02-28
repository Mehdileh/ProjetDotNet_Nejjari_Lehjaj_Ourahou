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
        private Task? task;

        public SetupService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope()) // Utilisation de IServiceScopeFactory
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

                // 🔥 Création d'un administrateur par défaut
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                string adminEmail = "nejjari@example.com";
                string adminPassword = "nejjari123";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = "Admin",
                        Email = adminEmail,
                        FirstName = "Nizar",
                        LastName = "Nejjari"
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        Console.WriteLine("✅ Administrateur créé !");
                    }
                    else
                    {
                        Console.WriteLine("🚨 Erreur lors de la création de l'administrateur :");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"❌ {error.Description}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ L'administrateur existe déjà.");
                }

                // 🔥 Ajout des catégories par défaut
                var defaultCategories = new List<Category>
                {
                    new Category { Name = "RPG" },
                    new Category { Name = "Open World" },
                    new Category { Name = "Racing" },
                    new Category { Name = "Shooter" },
                    new Category { Name = "Football" },
                    new Category { Name = "Survival" }
                };

                foreach (var category in defaultCategories)
                {
                    var existingCategory = await applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);
                    if (existingCategory == null)
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

                if (rpgCategory == null || openWorldCategory == null || racingCategory == null ||
                    shooterCategory == null || footballCategory == null || survivalCategory == null)
                {
                    Console.WriteLine("🚨 Erreur : Une ou plusieurs catégories n'ont pas été trouvées !");
                    return;
                }

                // 🔥 Ajout des jeux avec catégories
                var defaultGames = new List<Game>
                {
                    new Game { Name = "The Witcher 3", Description = "RPG Open World", Price = 39.99M, FilePath = "C:\\games\\witcher3.exe", Categories = new List<Category> { rpgCategory, openWorldCategory } },
                    new Game { Name = "Grand Theft Auto 6", Description = "RPG Open World", Price = 70.99M, FilePath = "C:\\games\\GTA6.exe", Categories = new List<Category> { rpgCategory, openWorldCategory } },
                    new Game { Name = "Red Dead Redemption", Description = "Texas and shit", Price = 50.00M, FilePath = "C:\\games\\RDR.exe", Categories = new List<Category> { openWorldCategory } },
                    new Game { Name = "Max Payne", Description = "Semi Open World", Price = 15.00M, FilePath = "C:\\games\\MP.exe", Categories = new List<Category> { shooterCategory } },
                    new Game { Name = "Forza Horizon 5", Description = "Racing Game", Price = 45.00M, FilePath = "C:\\games\\FH5.exe", Categories = new List<Category> { racingCategory } }
                };

                foreach (var game in defaultGames)
                {
                    var existingGame = await applicationDbContext.Games.FirstOrDefaultAsync(g => g.Name == game.Name);
                    if (existingGame == null)
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
    }
}
