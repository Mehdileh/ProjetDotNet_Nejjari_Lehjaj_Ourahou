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
                        FirstName = "Nizar",  // ✅ Ajout correct des valeurs
                        LastName = "Nejjari"  // ✅ Ajout correct des valeurs
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
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
