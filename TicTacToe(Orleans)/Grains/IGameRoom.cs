using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe_Orleans_.Grains
{
    public interface IGameRoom
    {
        public Task JoinRoom(string userId, string userName, string connectionId);
        public string X { get; set; }
        public string O { get; set; }
        public string Winner { get; set; }
       

        public List<List<char>> Board { get; set; }
        public List<string> Moves { get; set; }
    }
}