namespace Gauniv.Client.Models
{
    public class Game
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required string FilePath { get; set; }
        public List<string> Categories { get; set; } = new();

        public bool IsOwned { get; set; }

    }
}
