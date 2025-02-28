namespace Gauniv.Client.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string FilePath { get; set; }
        public List<string> Categories { get; set; } = new();
    }
}
