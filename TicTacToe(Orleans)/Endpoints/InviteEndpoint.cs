using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using TicTacToe_Orleans_.Model;
namespace TicTacToe_Orleans_.Endpoints
{
    public static class InviteEndpoints
    {
	public static void MapInviteEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Invite");

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            return await db.Invites.ToListAsync();
        })
        .WithName("GetAllInvites");

        group.MapGet("/{id}", async Task<Results<Ok<Invite>, NotFound>> (Guid id, ApplicationDbContext db) =>
        {
            return await db.Invites.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Invite model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetInviteById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, Invite invite, ApplicationDbContext db) =>
        {
            var affected = await db.Invites
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

        group.MapPost("/", async (Invite invite, ApplicationDbContext db) =>
        {
            db.Invites.Add(invite);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Invite/{invite.Id}",invite);
        })
        .WithName("CreateInvite");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, ApplicationDbContext db) =>
        {
            var affected = await db.Invites
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteInvite");
    }
    }
}
