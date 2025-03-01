using System.ComponentModel.DataAnnotations.Schema;

namespace Gauniv.WebServer.Data
{
    public class UserGame
    {
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int GameId { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }
    }
}
