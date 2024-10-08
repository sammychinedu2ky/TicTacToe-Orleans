﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicTacToe_Orleans.Authorization;
using TicTacToe_Orleans.Model;
namespace TicTacToe_Orleans.Endpoints
{
    public static class InvitationEndpoints
    {
        public class InvitationEndpoint { }
        public static void MapInviteEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/orleans/Invitations");


            group.MapGet("/my-invites", async Task<Ok<List<InvitationDTO>>> (ApplicationDbContext db, HttpContext context, ILogger<InvitationEndpoint> logger) =>
            {
                var identity = context?.User?.Identity as ClaimsIdentity;
                var email = identity?.FindFirst(ClaimTypes.Email)?.Value;
                var invites = await db.Invitations.Where(model => model.To == email && model.NewInvite == true).ToListAsync();
                return TypedResults.Ok(invites.ToDTO());
            }).RequireAuthorization(CookieHandlerRequirement.Policy);


            group.MapPut("/accept/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db, ILogger<InvitationEndpoint> logger) =>
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

            group.MapPut("/reject/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db, ILogger<InvitationEndpoint> logger) =>
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

            group.MapGet("/{id}", async Task<Results<Ok<Invitation>, NotFound>> (Guid id, ApplicationDbContext db, ILogger<InvitationEndpoint> logger) =>
            {
                return await db.Invitations.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is Invitation model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
            });
        }
    }
}
