using System.Collections.Generic;

namespace Gauniv.Client.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Relation avec les jeux (Un jeu peut appartenir à plusieurs catégories)
        public List<Game> Games { get; set; } = new();
    }
}
