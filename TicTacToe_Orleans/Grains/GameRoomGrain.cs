
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Orleans.Runtime;
using TicTacToe_Orleans.Hubs;
using TicTacToe_Orleans.Model;

namespace TicTacToe_Orleans.Grains
{
    public class GameRoomGrain : IGameRoomGrain, IDisposable
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IHubContext<GameRoomHub, IGameRoomClient> _hubContext;
        private readonly IGrainFactory? _grainFactory;
        private IGrainContext _grainContext;
        private Guid _grainId;

        private GameRoomType? _gameRoomType { get; set; }
        public GameRoomState State { get; set; } = new GameRoomState();

        public GameRoomGrain(IDbContextFactory<ApplicationDbContext> dbContextFactory,
            IHubContext<GameRoomHub, IGameRoomClient> hubContext,
            IGrainFactory grainFactory,
            IGrainContext grainContext)
        {
            _dbContextFactory = dbContextFactory;
            _hubContext = hubContext;
            _grainFactory = grainFactory;
            _grainContext = grainContext;
            _grainId = new Guid(_grainContext.GrainId.Key.ToString()!);
        }
        public async Task JoinGameRoom(string? userId, string connectionId)
        {
            if (_gameRoomType is null)
            {
                using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
                {
                    var gameRoom = await dbContext.GameRooms.FindAsync(_grainId);
                    if (gameRoom is not null)
                    {
                        _gameRoomType = gameRoom!.Type;
                        State.X = gameRoom.X;
                        State.O = gameRoom.O;

                    }
                    else
                    {
                        await _hubContext.Clients.Client(connectionId).ReceiveError("Can't join more than one room");
                    }
                }
            }
            var connectionGrain = _grainFactory!.GetGrain<IConnectionGrain>(nameof(MyConnections));
            if (!String.IsNullOrEmpty(userId))
            {
                var check = await connectionGrain.IsConnectedAsync(userId);
                if (!check)
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, _grainId.ToString());
                    await connectionGrain.AddUserAsync(userId, connectionId);
                    
                    try
                    {

                    await _hubContext.Clients.Client(connectionId).ReceiveGameState(State);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {

                    await _hubContext.Clients.Client(connectionId).ReceiveError( "Can't join more than one room");
                    return;
                }
            }
            else
            {
                await _hubContext.Groups.AddToGroupAsync(connectionId, _grainId.ToString());
                await connectionGrain.AddUserAsync(null, connectionId);
                await _hubContext.Clients.Client(connectionId).ReceiveGameState(State);
            }
        }
        public void AssignTurn()
        {
            if (State.Turn == "x")
            {
                State.Turn = "o";
            }
            else
            {
                State.Turn = "x";
            }
           
        }

        public async Task SendGameState(GameRoomState gameRoomState)
        {
            var player = gameRoomState.Turn;
            if (IsDraw(gameRoomState))
            {
                gameRoomState.Winner = "Draw";
                gameRoomState.Draw++;
                State = gameRoomState;
                await _hubContext.Clients.Group(_grainId.ToString()).ReceiveGameState(gameRoomState);
                return;
            }
            if (HasWon(gameRoomState))
            {
                gameRoomState.Winner = player;
                var winnersName = gameRoomState.Winner == "x" ? gameRoomState.X : gameRoomState.O;
                if (player == "x")
                {
                    gameRoomState.XWins++;
                }
                else
                {
                    gameRoomState.OWins++;
                }
                State = gameRoomState;
                await _hubContext.Clients.Group(_grainId.ToString()).ReceiveGameState(gameRoomState);
                return;
            }
            if (player ==  "x")
            {
                gameRoomState.Turn = "o";
            }
            else
            {
                gameRoomState.Turn = "x";
            }
           
            State = gameRoomState;
           await  _hubContext.Clients.Group(_grainId.ToString()).ReceiveGameState(gameRoomState);
        }

        private bool HasWon(GameRoomState gameRoomState)
        {
            var board = gameRoomState.Board;
            // horizontal
            for (var i = 0; i < board.Count; i++)
            {
                if (board[i][0] != String.Empty && board[i][0] == board[i][1] && board[i][1] == board[i][2])
                {
                    return true;
                }
            }
            // vertical
            for (var i = 0; i < board.Count; i++)
            {
                if (board[0][i] != String.Empty & board[0][i] == board[1][i] && board[1][i] == board[2][i])
                {
                    return true;
                }
            }
            // diagonal
            if (board[0][0] != String.Empty & board[0][0] == board[1][1] && board[1][1] == board[2][2])
            {
                return true;
            }
            if (board[0][2] != string.Empty && board[0][2] == board[1][1] && board[1][1] == board[2][0])
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
                    if (board[i][j] == String.Empty)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private async Task SaveToDb()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var gameRoom = await dbContext.GameRooms.FindAsync(this.GetPrimaryKey());
                gameRoom.X = State.X;
                gameRoom.O = State.O;
                gameRoom.XWins = State.XWins;
                gameRoom.OWins = State.OWins;
                gameRoom.Draw = State.Draw;
                gameRoom.Type = _gameRoomType!.Value;
                await dbContext.SaveChangesAsync();

            }
        }

        public async Task PlayAgain()
        {
            List<List<string>> board =  new()
        {
            new List<string> { "", "", "" },
            new List<string> { "", "", "" },
            new List<string> { "", "", "" }
        };
        State.Board = board;
            State.Turn = "x";
            await _hubContext.Clients.Group(this.GetPrimaryKey().ToString()).ReceiveGameState(State);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
