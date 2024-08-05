namespace TicTacToe_Orleans.Grains
{
    public interface IGameRoomGrain : IGrainWithGuidKey
    {
         Task JoinRoom(string? userId, string? userName, string connectionId);
       
    }
}