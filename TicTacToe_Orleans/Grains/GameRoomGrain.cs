
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TicTacToe_Orleans.Hubs;
using TicTacToe_Orleans.Model;

namespace TicTacToe_Orleans.Grains
{
    public class GameRoomGrain : IGameRoomGrain
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IHubContext<GameRoomHub, IGameRoomClient> _hubContext;
        private readonly IGrainFactory? _grainFactory;
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
        public async Task JoinRoom(string? userId, string connectionId)
        {
            if (_gameRoomType is null)
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    var gameRoom = await dbContext.GameRooms.FindAsync(this.GetPrimaryKey());
                    _gameRoomType = gameRoom!.Type;
                }
            }
            var connectionGrain = _grainFactory!.GetGrain<IConnectionGrain>(userId);
            if (!String.IsNullOrEmpty(userId))
            {
                var check = await connectionGrain.IsConnectedAsync(userId);
                if (!check)
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, this.GetPrimaryKey().ToString());
                    await connectionGrain.AddUserAsync(userId, connectionId);
                    AssignState(userId);
                    await _hubContext.Clients.Client(connectionId).ReceiveGameState(State);
                }
                else
                {

                    await _hubContext.Clients.Client(connectionId).ReceiveError(connectionId, "Can't join more than one room");
                    return;
                }
            }
            else
            {
                await _hubContext.Groups.AddToGroupAsync(connectionId, this.GetPrimaryKey().ToString());
                await connectionGrain.AddUserAsync(null, connectionId);
                await _hubContext.Clients.Client(connectionId).ReceiveGameState(State);
            }
        }
        public void AssignState(string userId)
        {
            if (State.X is null)
            {
                State.X = userId;
                if (_gameRoomType == GameRoomType.Computer)
                {
                    State.O = "Computer";
                }
            }
            else State.O = userId;
        }

        public async Task SendGameState(GameRoomState gameRoomState)
        {
            var player = gameRoomState.Turn;
            if (IsDraw(gameRoomState))
            {
                gameRoomState.Winner = "Draw";
                gameRoomState.Draw++;
                State = gameRoomState;
                await _hubContext.Clients.Group(this.GetPrimaryKey().ToString()).ReceiveGameState(gameRoomState);
            }
            if (HasWon(gameRoomState))
            {
                gameRoomState.Winner = player.ToString();
                if (player == 'X')
                {
                    gameRoomState.XWins++;
                }
                else
                {
                    gameRoomState.OWins++;
                }
                State = gameRoomState;
                await _hubContext.Clients.Group(this.GetPrimaryKey().ToString()).ReceiveGameState(gameRoomState);
            }
            if (player == 'X')
            {
                gameRoomState.Turn = 'O';
            }
            else
            {
                gameRoomState.Turn = 'X';
            }
            State = gameRoomState;
           await  _hubContext.Clients.Group(this.GetPrimaryKey().ToString()).ReceiveGameState(gameRoomState);
        }

        private bool HasWon(GameRoomState gameRoomState)
        {
            var board = gameRoomState.Board;
            // horizontal
            for (var i = 0; i < board.Count; i++)
            {
                if (board[i][0] == board[i][1] && board[i][1] == board[i][2])
                {
                    return true;
                }
            }
            // vertical
            for (var i = 0; i < board.Count; i++)
            {
                if (board[0][i] == board[1][i] && board[1][i] == board[2][i])
                {
                    return true;
                }
            }
            // diagonal
            if (board[0][0] == board[1][1] && board[1][1] == board[2][2])
            {
                return true;
            }
            if (board[0][2] == board[1][1] && board[1][1] == board[2][0])
            {
                return true;
            }
            return false;
        }

        private bool IsDraw(GameRoomState gameRoomState)
        {
            var board = gameRoomState.Board;
            for (var i = 0; i < board.Count; i++)
            {
                for (var j = 0; j < board[i].Count; j++)
                {
                    if (board[i][j] == ' ')
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private async Task SaveToDb(GameRoomState goomRoomState)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var gameRoom = await dbContext.GameRooms.FindAsync(this.GetPrimaryKey());
                gameRoom.X = goomRoomState.X;
                gameRoom.O = goomRoomState.O;
                gameRoom.XWins = goomRoomState.XWins;
                gameRoom.OWins = goomRoomState.OWins;
                gameRoom.Draw = goomRoomState.Draw;
                gameRoom.Type = _gameRoomType!.Value;
                await dbContext.SaveChangesAsync();

            }
        }

        public async Task PlayAgain()
        {
            var board = new List<List<char>>
            {
                new List<char> { ' ', ' ', ' ' },
                new List<char> { ' ', ' ', ' ' },
                new List<char> { ' ', ' ', ' ' }
            };
            State.Board = board;
            State.Turn = 'X';
            await _hubContext.Clients.Group(this.GetPrimaryKey().ToString()).ReceiveGameState(State);
        }
    }
}
