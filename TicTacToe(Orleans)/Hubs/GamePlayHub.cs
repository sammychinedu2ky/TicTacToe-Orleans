using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Polly;
using System.Security.Claims;

namespace TicTacToe_Orleans_.Hubs
{
    public class GamePlayHub : Hub
    {
        private DbContext _dbContext;

        public GamePlayHub(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task JoinRoom(Guid roomId)
        {
            // check if a gameplay exists

            var identity = Context.User.Identity as ClaimsIdentity;
            var email = identity.FindFirst(ClaimTypes.Email)?.Value;
            var name = identity.FindFirst(ClaimTypes.Name)?.Value;

        }
    }
}
