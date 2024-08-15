using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using TicTacToe_Orleans.Model;
using TicTacToe_Orleans.Authorization;
using System.Security.Claims;
namespace TicTacToe_Orleans.Endpoints
{
    public static class UserEndpoints
    {
        public class UserEndpoint { }
        public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/User");

            group.MapPost("/", async Task<Results<Created<User>, ProblemHttpResult>> (User user, ApplicationDbContext db, ILogger<UserEndpoint> logger) =>
            {
                try
                {
                    var userExists = await db.Users
                        .Where(model => model.Id == user.Id)
                        .AnyAsync();

                    if (!userExists)
                    {
                        db.Users.Add(user);
                        await db.SaveChangesAsync();
                    }

                    return TypedResults.Created($"/api/User/{user.Id}", user);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex.Message);
                    return TypedResults.Problem("An error occurred while processing your request.");
                }

            })
            .RequireAuthorization(AuthSecretRequirement.Policy);

            group.MapGet("/", async Task<Results<Ok<List<User>>, ProblemHttpResult>> (ApplicationDbContext db, HttpContext context, ILogger<UserEndpoint> logger) =>
            {
                try
                {
                return TypedResults.Ok(await db.Users.ToListAsync());
                }
                catch(Exception ex)
                {
                    logger.LogError(ex.Message);
                    return TypedResults.Problem("An error occurred while processing your request.");
                }

            })
            .WithName("GetAllUsers").RequireAuthorization(CookieHandlerRequirement.Policy);

            group.MapGet("/{id}", async Task<Results<Ok<User>, NotFound, ProblemHttpResult>> (string id, ApplicationDbContext db, ILogger<UserEndpoint> logger) =>
            {
                try
                {
                return await db.Users.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is User model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
                }
                catch(Exception ex)
                {
                    logger.LogError(ex.Message);
                    return   TypedResults.Problem("An error occurred while processing your request.");
                }
            })
           .RequireAuthorization(CookieHandlerRequirement.Policy);

        }
    }
}
