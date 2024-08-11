namespace TicTacToe_Orleans.Grains
{
    public interface IGameRoomGrain : IGrainWithGuidKey
    {
         Task JoinRoom(string? userId, string connectionId);
         Task SendGameState(string? userId, string connectionId, GameRoomState gameRoomState);
    }
}