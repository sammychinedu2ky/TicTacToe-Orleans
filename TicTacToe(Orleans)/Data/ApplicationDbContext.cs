using Microsoft.EntityFrameworkCore;
using TicTacToe_Orleans_.Model;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<GameRoom> GameRooms { get; set; } = default!;

    public DbSet<User> Users { get; set; } = default!;

    public DbSet<Invite> Invites { get; set; } = default!;
   
}
