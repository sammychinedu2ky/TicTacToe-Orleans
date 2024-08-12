namespace TicTacToe_Orleans.Grains
{
    public interface IGameRoomGrain : IGrainWithGuidKey
    {
        Task JoinGameRoom(string? userId, string connectionId);
        Task SendGameState(GameRoomState gameRoomState);
        Task PlayAgain();
    }
}