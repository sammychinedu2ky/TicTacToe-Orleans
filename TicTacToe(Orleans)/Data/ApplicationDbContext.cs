using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicTacToe_Orleans_.Model;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<GamePlay> GamePlay { get; set; } = default!;

    public DbSet<User> User { get; set; } = default!;

    public DbSet<Invite> Invite { get; set; } = default!;
   
}
