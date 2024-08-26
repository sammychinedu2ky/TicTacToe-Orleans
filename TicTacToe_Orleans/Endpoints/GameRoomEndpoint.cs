using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicTacToe_Orleans.Authorization;
using TicTacToe_Orleans.Hubs;
using TicTacToe_Orleans.Model;
namespace TicTacToe_Orleans.Endpoints
{
    public static class GameRoomEndpoints
    {
        public class GameRoomEndpoint { }
        public static void MapGameRoomEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/orleans/game-room");


            group.MapGet("/{id}", async Task<Results<Ok<GameRoom>, NotFound>> (Guid id, ApplicationDbContext db, ILogger<GameRoomEndpoint> logger) =>
            {
                return await db.GameRooms.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is GameRoom model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
            }).RequireAuthorization(CookieHandlerRequirement.Policy);



            group.MapPost("/", async Task<Created<GameRoom>> (GameRoomDto gameRoomDto,
                ApplicationDbContext db,
                IHubContext<GameRoomHub, IGameRoomClient> hubContext,
                HttpContext context,
                ILogger<GameRoomEndpoint> logger) =>
            {
                var identity = context.User.Identity as ClaimsIdentity;
                var user = identity!.FindFirst(ClaimTypes.Email)!.Value!;
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
                else
                {
                    gameRoom.O = "Computer";
                }
                db.GameRooms.Add(gameRoom);
                await db.SaveChangesAsync();
                if (invitation is not null)
                {
                    await hubContext.Clients.Group(gameRoomDto.Email).ReceiveInvite(invitation.ToDTO());
                }
                return TypedResults.Created($"/api/GamePlay/{gameRoom.Id}", gameRoom);
            })
            .RequireAuthorization(CookieHandlerRequirement.Policy);

        }

    }

    internal class GameRoomDto
    {
        public Guid Id { get; set; }
        public GameRoomType Type { get; set; }
        public string Email { get; set; } = default!;
    }
}
