using Microsoft.EntityFrameworkCore;
using TicTacToe_Orleans.Model;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<GameRoom> GameRooms { get; set; } = default!;

    public DbSet<User> Users { get; set; } = default!;

    public DbSet<Invitation> Invites { get; set; } = default!;
   
}
