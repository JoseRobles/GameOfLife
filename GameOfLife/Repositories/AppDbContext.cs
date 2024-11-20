using GameOfLife.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Board> Boards { get; set; }
    public DbSet<Coordinate> Coordinate { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar la relación entre Board y Coordinate
        modelBuilder.Entity<Board>()
                   .HasMany(b => b.coordinates)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Restrict);
    }
}