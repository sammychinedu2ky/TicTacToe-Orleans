using TicTacToe_Orleans.Grains;
using TicTacToe_Orleans.Model;
namespace TicTacToe_Orleans.Hubs
{
    public interface IGameRoomClient
    {
        Task ReceiveInvite(InvitationDTO invite);
        Task ReceiveGameState(GameRoomState gameRoomState);
        Task ReceiveError(string connectionId, string message);
    }

  
}