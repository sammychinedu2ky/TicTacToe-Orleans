using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using TicTacToe_Orleans.Model;
using Polly;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication.Cookies;
using TicTacToe_Orleans.Authorization;
namespace TicTacToe_Orleans.Endpoints
{
    public static class InviteEndpoints
    {
        public static void MapInviteEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Invitations");

            group.MapGet("/", async (ApplicationDbContext db) =>
            {
                return await db.Invitations.ToListAsync();
            })
            .WithName("GetAllInvites").RequireAuthorization(CookieHandlerRequirement.Policy);
            group.MapGet("/my-invites", async (ApplicationDbContext db, HttpContext context) =>
            {
                var identity = context?.User?.Identity as ClaimsIdentity;
                var email = identity?.FindFirst(ClaimTypes.Email)?.Value;
                var invites = await db.Invitations.Where(model => model.To == email && model.NewInvite==true).ToListAsync();

                return invites.ToDTO();
            }).RequireAuthorization(CookieHandlerRequirement.Policy);


            group.MapPut("/accept/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) =>
            {
                var invite = await db.Invitations.FirstOrDefaultAsync(model => model.Id == id);
                if (invite is null)
                {
                    return TypedResults.NotFound();
                }
                invite.Accept = true;
                invite.NewInvite = false;
                await db.SaveChangesAsync();
                return TypedResults.Ok();
            }).RequireAuthorization(CookieHandlerRequirement.Policy);

            group.MapPut("/reject/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) =>
            {
                var invite = await db.Invitations.FirstOrDefaultAsync(model => model.Id == id);
                if (invite is null)
                {
                    return TypedResults.NotFound();
                }
                invite.Accept = false;
                invite.NewInvite = false;
                await db.SaveChangesAsync();
                return TypedResults.Ok();
            }).RequireAuthorization(CookieHandlerRequirement.Policy);
            group.MapGet("/{id}", async Task<Results<Ok<Invitation>, NotFound>> (Guid id, ApplicationDbContext db) =>
            {
                return await db.Invitations.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is Invitation model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
            })
            .WithName("GetInviteById");

            group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, Invitation invite, ApplicationDbContext db) =>
            {
                var affected = await db.Invitations
                    .Where(model => model.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                      .SetProperty(m => m.Id, invite.Id)
                      .SetProperty(m => m.From, invite.From)
                      .SetProperty(m => m.To, invite.To)
                      .SetProperty(m => m.GameRoom, invite.GameRoom)
                      .SetProperty(m => m.NewInvite, invite.NewInvite)
                      .SetProperty(m => m.Accept, invite.Accept)
                      );
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("UpdateInvite");

            group.MapPost("/", async (Invitation invite, ApplicationDbContext db) =>
            {
                db.Invitations.Add(invite);
                await db.SaveChangesAsync();
                return TypedResults.Created($"/api/Invite/{invite.Id}", invite);
            })
            .WithName("CreateInvite");

            group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) =>
            {
                var affected = await db.Invitations
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync();
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("DeleteInvite");
        }
    }
}
