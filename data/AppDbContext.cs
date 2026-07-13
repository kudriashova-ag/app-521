namespace Data;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<Director> Directors { get; set; } = null!;
    public DbSet<Actor> Actors { get; set; } = null!;
    public DbSet<MovieActor> MovieActors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // movie -> actor    many-to-many
        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId }); // PRIMARY KEY (movieId, actorId)

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Movie)
            .WithMany(m => m.MovieActors)
            .HasForeignKey(ma => ma.MovieId);

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Actor)
            .WithMany(a => a.MovieActors)
            .HasForeignKey(ma => ma.ActorId);


        // movie->director  one - to - many
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.Director) // у Movie є одна навігаційна властивість Director
            .WithMany(d => d.Movies) // у Director є навігаційна колекція Movies
            .HasForeignKey(m => m.DirectorId);
    }
}