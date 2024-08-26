using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TicTacToe_Orleans.Authorization;
using TicTacToe_Orleans.Model;
namespace TicTacToe_Orleans.Endpoints
{
    public static class UserEndpoints
    {
        public class UserEndpoint { }
        public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/orleans/User");

            group.MapPost("/", async Task<Created<User>> (User user, ApplicationDbContext db, ILogger<UserEndpoint> logger) =>
            {
                var userExists = await db.Users
                    .Where(model => model.Id == user.Id)
                    .AnyAsync();

                if (!userExists)
                {
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                }

                return TypedResults.Created($"/api/orleans/User/{user.Id}", user);

            })
            .RequireAuthorization(AuthSecretRequirement.Policy);

            group.MapGet("/", async Task<Ok<List<User>>> (ApplicationDbContext db, HttpContext context, ILogger<UserEndpoint> logger) =>
            {
                return TypedResults.Ok(await db.Users.ToListAsync());

            })
            .WithName("GetAllUsers").RequireAuthorization(CookieHandlerRequirement.Policy);

            group.MapGet("/{id}", async Task<Results<Ok<User>, NotFound>> (string id, ApplicationDbContext db, ILogger<UserEndpoint> logger) =>
            {
                return await db.Users.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is User model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
            })
           .RequireAuthorization(CookieHandlerRequirement.Policy);
        }
    }
}
