namespace TicTacToe_Orleans.Grains
{
    [GenerateSerializer]
    public class GameRoomState
    {
        public string? X { get; set; }
        public string? O { get; set; }
        public string? Winner { get; set; }
        public char Turn { get; set; } = 'X';
        public List<List<char>> Board { get; set; } = new List<List<char>>
        {
            new List<char> { ' ', ' ', ' ' },
            new List<char> { ' ', ' ', ' ' },
            new List<char> { ' ', ' ', ' ' }
        };
        public List<string> Moves { get; set; } = [];
    }
}
