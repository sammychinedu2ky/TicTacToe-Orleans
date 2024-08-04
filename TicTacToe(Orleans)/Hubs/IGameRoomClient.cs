﻿using TicTacToe_Orleans_.Grains;

namespace TicTacToe_Orleans_.Hubs
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