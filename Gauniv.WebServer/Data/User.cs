using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Gauniv.WebServer.Data
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Liste des jeux possédés par l'utilisateur
        public List<Game> OwnedGames { get; set; } = new();
    }
}
