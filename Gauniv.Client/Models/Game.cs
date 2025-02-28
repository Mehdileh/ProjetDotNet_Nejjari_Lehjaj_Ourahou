namespace Gauniv.Client.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new();
    }
}
