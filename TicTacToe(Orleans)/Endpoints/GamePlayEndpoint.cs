﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using TicTacToe_Orleans_.Model;
namespace TicTacToe_Orleans_.Endpoints
{



public static class GamePlayEndpoints
{
	public static void MapGamePlayEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/GamePlay");

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            return await db.GamePlay.ToListAsync();
        })
        .WithName("GetAllGamePlays");

        group.MapGet("/{id}", async Task<Results<Ok<GamePlay>, NotFound>> (Guid id, ApplicationDbContext db) =>
        {
            return await db.GamePlay.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is GamePlay model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetGamePlayById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, GamePlay gamePlay, ApplicationDbContext db) =>
        {
            var affected = await db.GamePlay
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, gamePlay.Id)
                  .SetProperty(m => m.X, gamePlay.X)
                  .SetProperty(m => m.O, gamePlay.O)
                  .SetProperty(m => m.Winner, gamePlay.Winner)
                  .SetProperty(m => m.Board, gamePlay.Board)
                  .SetProperty(m => m.Moves, gamePlay.Moves)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateGamePlay");

        group.MapPost("/", async (GamePlay gamePlay, ApplicationDbContext db) =>
        {
            db.GamePlay.Add(gamePlay);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/GamePlay/{gamePlay.Id}",gamePlay);
        })
        .WithName("CreateGamePlay");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) =>
        {
            var affected = await db.GamePlay
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteGamePlay");
    }
	
}}
