namespace TicTacToe_Orleans_.Grains
{
    public interface IGameRoomGrain : IGrainWithGuidKey
    {
        public Task JoinRoom(string? userId, string? userName, string connectionId);
       
    }
}