using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using TicTacToe_Orleans_.Model;
using TicTacToe_Orleans_.Authorization;
namespace TicTacToe_Orleans_.Endpoints
{
public static class UserEndpoints
{
	public static void MapUserEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/User");

        group.MapPost("/", async (User user, ApplicationDbContext db) =>
        {
           // check if user already exists
           var userExists = await db.User
                .Where(model => model.Id == user.Id)
                .AnyAsync();
            if (!userExists)
            {
            db.User.Add(user);
            await db.SaveChangesAsync();
            }
            return TypedResults.Created($"/api/User/{user.Id}",user);
        })
        .WithName("CreateUser")
        .RequireAuthorization(AuthSecretRequirement.Policy);

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            return await db.User.ToListAsync();
        })
        .WithName("GetAllUsers");

        group.MapGet("/{id}", async Task<Results<Ok<User>, NotFound>> (string id, ApplicationDbContext db) =>
        {
            return await db.User.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is User model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetUserById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (string id, User user, ApplicationDbContext db) =>
        {
            var affected = await db.User
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, user.Id)
                  .SetProperty(m => m.Name, user.Name)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateUser");


        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (string id, ApplicationDbContext db) =>
        {
            var affected = await db.User
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteUser");
    }
}}
