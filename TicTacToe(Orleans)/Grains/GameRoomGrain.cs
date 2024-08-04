
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TicTacToe_Orleans_.Hubs;
using TicTacToe_Orleans_.Model;

namespace TicTacToe_Orleans_.Grains
{
    public class GameRoomGrain : IGameRoomGrain
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IHubContext<GameRoomHub, IGameRoomClient> _hubContext;
        private readonly IGrainFactory _grainFactory;
        private GameRoomType? _gameRoomType { get; set; }
        public GameRoomState State { get; set; } = new GameRoomState();

        public GameRoomGrain(IDbContextFactory<ApplicationDbContext> dbContextFactory,
            IHubContext<GameRoomHub, IGameRoomClient> hubContext,
            IGrainFactory grainFactory)
        {
            _dbContextFactory = dbContextFactory;
            _hubContext = hubContext;
            this._grainFactory = grainFactory;
        }
        public async Task JoinRoom(string userId, string userName, string connectionId)
        {
            if (_gameRoomType is null)
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    var gameRoom = await dbContext.GameRooms.FindAsync(this.GetPrimaryKey());
                    _gameRoomType = gameRoom!.Type;
                }
            }
            var connectionGrain = _grainFactory.GetGrain<IConnectionGrain>(userId);
            if (!String.IsNullOrEmpty(userId))
            {
                var check = await connectionGrain.IsConnectedAsync(userId);
                if (!check)
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, this.GetPrimaryKey().ToString());
                    await connectionGrain.AddUserAsync(userId, connectionId);
                    AssignState(userId);
                    await _hubContext.Clients.Client(connectionId).ReceiveGameStateAsync(this.GetPrimaryKey(), State);
                }
                else
                {

                    await _hubContext.Clients.Client(connectionId).ReceiveErrorAsync(connectionId, "Can't join more than one room");
                    return;
                }
            }
            else
            {
                await _hubContext.Groups.AddToGroupAsync(connectionId, this.GetPrimaryKey().ToString());
                await connectionGrain.AddUserAsync(null, connectionId);

            }
        }
        public void AssignState(string userId)
        {
            if (State.X is null)
            {
                State.X = userId;
            }
            else State.O = userId;
        }
    }
}
