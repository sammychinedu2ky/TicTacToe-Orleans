using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using TicTacToe_Orleans_.Model;
namespace TicTacToe_Orleans_.Endpoints
{



public static class GameRoomEndpoints
{
	public static void MapGameRoomEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/GamePlay");

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
                  .SetProperty(m => m.Winner, gamePlay.Winner)
                  .SetProperty(m => m.Board, gamePlay.Board)
                  .SetProperty(m => m.Moves, gamePlay.Moves)
                  .SetProperty(m => m.Type, gamePlay.Type)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateGameRoom");

        group.MapPost("/", async (GameRoom gamePlay, ApplicationDbContext db) =>
        {
            db.GameRooms.Add(gamePlay);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/GamePlay/{gamePlay.Id}",gamePlay);
        })
        .WithName("CreateGameRoom");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) =>
        {
            var affected = await db.GameRooms
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteGamePlay");
    }
	
}}
