namespace TicTacToe_Orleans.Grains
{
    [GenerateSerializer]
    public class GameRoomState
    {
        public string? X { get; set; }
        public string? O { get; set; }
        public string? Winner { get; set; }
        public char Turn { get; set; } = 'X';
        public int XWins { get; set; }
        public int OWins { get; set; }
        public int Draw { get; set; }
        public List<List<char>> Board { get; set; } = new List<List<char>>
        {
            new List<char> { ' ', ' ', ' ' },
            new List<char> { ' ', ' ', ' ' },
            new List<char> { ' ', ' ', ' ' }
        };
    }
}
