using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Gauniv.WebServer.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public List<Game> Games { get; set; } = new();
    }
}
