using TicTacToe_Orleans.Grains;
using TicTacToe_Orleans.Model;
namespace TicTacToe_Orleans.Hubs
{
    public interface IGameRoomClient
    {
        Task ReceiveInviteAsync(string userId, InvitationDTO invite);
        Task ReceiveGameStateAsync(Guid game, GameRoomState gameRoomState);
        Task ReceiveErrorAsync(string connectionId, string message);
    }

  
}