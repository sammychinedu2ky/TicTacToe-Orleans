using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using TicTacToe_Orleans.Model;
using Microsoft.AspNetCore.SignalR;
using TicTacToe_Orleans.Hubs;
using TicTacToe_Orleans.Authorization;
using Polly;
using System.Security.Claims;
namespace TicTacToe_Orleans.Endpoints
{
    public static class GameRoomEndpoints
    {
        public static void MapGameRoomEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/game-room");

            group.MapGet("/", async (ApplicationDbContext db) =>
            {
                return await db.GameRooms.ToListAsync();
            })
            .WithName("GetAllGameRooms");

            group.MapGet("/{id}", async Task<Results<Ok<GameRoom>, NotFound>> (Guid id, ApplicationDbContext db) =>
            {
                return await db.GameRooms.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is GameRoom model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
            })
            .WithName("GetGameRoomById");

            group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, GameRoom gamePlay, ApplicationDbContext db) =>
            {
                var affected = await db.GameRooms
                    .Where(model => model.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                      .SetProperty(m => m.Id, gamePlay.Id)
                      .SetProperty(m => m.X, gamePlay.X)
                      .SetProperty(m => m.O, gamePlay.O)
                      .SetProperty(m => m.Type, gamePlay.Type)
                      );
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("UpdateGameRoom");

            group.MapPost("/", async (GameRoomDto gameRoomDto, ApplicationDbContext db, IHubContext<GameRoomHub, IGameRoomClient> hubContext, HttpContext context) =>
            {
                var identity = context?.User?.Identity as ClaimsIdentity;
                var user = identity?.FindFirst(ClaimTypes.Email)?.Value;
                var gameRoom = new GameRoom
                {
                    Id = gameRoomDto.Id,
                    Type = gameRoomDto.Type,
                    X = user,
                    O = gameRoomDto.Email
                };
                Invitation? invitation = null;
                if (!String.IsNullOrEmpty(gameRoomDto.Email))
                {
                    invitation = new Invitation
                    {
                        Id = gameRoom.Id,
                        From = user!,
                        To = gameRoomDto.Email,
                        GameRoom = gameRoom.Id,
                        NewInvite = true
                    };
                    db.Invitations.Add(invitation);

                }
                db.GameRooms.Add(gameRoom);
                await db.SaveChangesAsync();
                if (invitation is not null)
                {
                    await hubContext.Clients.Group(gameRoomDto.Email).ReceiveInvite(invitation.ToDTO());
                }
                return TypedResults.Created($"/api/GamePlay/{gameRoom.Id}", gameRoom);
            })
            .WithName("CreateGameRoom").RequireAuthorization(CookieHandlerRequirement.Policy);
            group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) =>
            {
                var affected = await db.GameRooms
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync();
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("DeleteGamePlay");
        }

    }

    internal class GameRoomDto
    {
        public Guid Id { get; set; }
        public GameRoomType Type { get; set; }
        public string Email { get; set; }
    }
}
