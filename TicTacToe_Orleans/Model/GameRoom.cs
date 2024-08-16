using System.Text.Json.Serialization;
namespace TicTacToe_Orleans.Model
{
    public class GameRoom
    {
        public Guid Id { get; set; }
        public string X { get; set; } = string.Empty;
        public string O { get; set; } = string.Empty;


        //[Column("Board", TypeName = "json")]
        //public List<List<char>> Board { get; set; } = new List<List<char>>
        //{
        //    new List<char> { ' ', ' ', ' ' },
        //    new List<char> { ' ', ' ', ' ' },
        //    new List<char> { ' ', ' ', ' ' }
        //};
        public int XWins { get; set; }
        public int OWins { get; set; }
        public int Draw { get; set; }
        public GameRoomType Type { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameRoomType
    {
        Computer,
        User
    }
}
