using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using TicTacToe_Orleans_.Model;
using TicTacToe_Orleans_.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
namespace TicTacToe_Orleans_.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/User");

            group.MapPost("/", async (User user, ApplicationDbContext db) =>
            {
                // check if user already exists
                var userExists = await db.Users
                 .Where(model => model.Id == user.Id)
                 .AnyAsync();
                if (!userExists)
                {
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                }
                return TypedResults.Created($"/api/User/{user.Id}", user);
            })
            .WithName("CreateUser")
            .RequireAuthorization(AuthSecretRequirement.Policy);

            group.MapGet("/", async (ApplicationDbContext db, HttpContext context) =>
            {
                var identity = context.User.Identity as ClaimsIdentity;
                var email = identity.FindFirst(ClaimTypes.Email)?.Value;
                var name = identity.FindFirst(ClaimTypes.Name)?.Value;
               
                return await db.Users.ToListAsync();
            })
            .WithName("GetAllUsers").RequireAuthorization(JwtBearerDefaults.AuthenticationScheme);

            group.MapGet("/{id}", async Task<Results<Ok<User>, NotFound>> (string id, ApplicationDbContext db) =>
            {
                return await db.Users.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is User model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
            })
            .WithName("GetUserById");

            group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (string id, User user, ApplicationDbContext db) =>
            {
                var affected = await db.Users
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
                var affected = await db.Users
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync();
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("DeleteUser");
        }
    }
}
