
namespace TicTacToe_Orleans_.Grains
{
    public class GameRoom : IGameRoom
    {
        public string X { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string O { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Winner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<List<char>> Board { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<string> Moves { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public Task JoinRoom(string userId, string userName, string connectionId)
        {
            throw new NotImplementedException();
        }
    }
}
