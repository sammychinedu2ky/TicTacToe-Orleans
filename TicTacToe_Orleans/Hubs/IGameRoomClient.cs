using TicTacToe_Orleans.Grains;

namespace TicTacToe_Orleans.Hubs
{
    public interface IGameRoomClient
    {
        Task ReceiveInviteAsync(string userId, InviteDto invite);
        Task ReceiveGameStateAsync(Guid game, GameRoomState gameRoomState);
        Task ReceiveErrorAsync(string connectionId, string message);
    }

    public class InviteDto
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public Guid GameRoom { get; set; }
    }
}