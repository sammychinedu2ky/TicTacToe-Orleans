namespace TicTacToe_Orleans.Grains
{
    [GenerateSerializer]
    public class GameRoomState
    {
        [Id(0)]
        public string? X { get; set; }
        [Id(1)]
        public string? O { get; set; }
        [Id(2)]
        public string? Winner { get; set; }
        [Id(3)]
        public string Turn { get; set; } = "x";
        [Id(4)]
        public int XWins { get; set; }
        [Id(5)]
        public int OWins { get; set; }
        [Id(6)]
        public int Draw { get; set; }
        [Id(7)]
        public List<List<string>> Board { get; set; } = new()
        {
            new List<string> { "", "", "" },
            new List<string> { "", "", "" },
            new List<string> { "", "", "" }
        };
    }
}
