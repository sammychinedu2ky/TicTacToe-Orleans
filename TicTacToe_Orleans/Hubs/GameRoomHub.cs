﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Polly;
using System.Security.Claims;
using TicTacToe_Orleans.Grains;

namespace TicTacToe_Orleans.Hubs
{
    public class MyConnections { };
    public class GameRoomHub : Hub<IGameRoomClient>
    {
        private ApplicationDbContext _dbContext;
        private IGrainFactory _grainFactory;

        public GameRoomHub(ApplicationDbContext dbContext, IGrainFactory grainFactory)
        {
            _dbContext = dbContext;
            _grainFactory = grainFactory;
        }
        public async Task JoinRoom(Guid roomId)
        {
            // check if a gameplay exists
            var exists = await _dbContext.GameRooms.AnyAsync(x => x.Id == roomId);
            if (!exists)
            {
                await Clients.Caller.ReceiveErrorAsync(Context.ConnectionId, "Game room does not exist");
                return;
            }
            var identity = Context?.User?.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;
            var name = identity?.FindFirst(ClaimTypes.Name)?.Value;
            var gameRoomGrain = _grainFactory.GetGrain<IGameRoomGrain>(roomId);
            await gameRoomGrain.JoinRoom(email, name, Context!.ConnectionId);

        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User!.Identity!.IsAuthenticated)
            {
                var identity = Context?.User?.Identity as ClaimsIdentity;
                var email = identity?.FindFirst(ClaimTypes.Email)?.Value!;

                await Groups.AddToGroupAsync(Context!.ConnectionId, email);
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (!Context.User!.Identity!.IsAuthenticated)
            {
                var connectionGrain = _grainFactory.GetGrain<IConnectionGrain>(nameof(MyConnections));
                await connectionGrain.RemoveUserAsync(null, Context.ConnectionId);
            }
            else
            {
                var identity = Context?.User?.Identity as ClaimsIdentity;
                var email = identity?.FindFirst(ClaimTypes.Email)?.Value!;
                var connectionId = Context.ConnectionId;
                var connectionGrain = _grainFactory.GetGrain<IConnectionGrain>(nameof(MyConnections));
                await connectionGrain.RemoveUserAsync(email, connectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}