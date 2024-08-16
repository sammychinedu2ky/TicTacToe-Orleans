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

    public DbSet<Invitation> Invitations { get; set; } = default!;

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    var boardConverter = new ValueConverter<List<List<char>>, string>(
    //             v => JsonSerializer.Serialize(v, null as JsonSerializerOptions), // Convert List<List<char>> to JSON string
    //             v => JsonSerializer.Deserialize<List<List<char>>>(v, null as JsonSerializerOptions) // Convert JSON string back to List<List<char>>
    //         );

    //    modelBuilder.Entity<GameRoom>()
    //        .Property(e => e.Board)
    //        .HasConversion(boardConverter);
    //    base.OnModelCreating(modelBuilder);
    //}

}
