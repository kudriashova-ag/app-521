using Bogus;
using Data;
using MyApp.Models;

public class SeedData
{
    public static void Initialize(AppDbContext context)
    {

        if (!context.Clients.Any())
        {
            var clients = new Faker<Client>()
                .RuleFor(c => c.FirstName, f => f.Name.FirstName())
                .RuleFor(c => c.LastName, f => f.Name.LastName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .Generate(50);

            context.Clients.AddRange(clients);
            context.SaveChanges();
        }

        if (!context.Movies.Any())
        {
            // add directors
            var directors = new Faker<Director>()
                .RuleFor(d => d.FirstName, f => f.Name.FirstName())
                .RuleFor(d => d.LastName, f => f.Name.LastName())
                .Generate(10);

            context.Directors.AddRange(directors);
            context.SaveChanges();

            // add actors
            var actors = new Faker<Actor>()
                .RuleFor(a => a.FirstName, f => f.Name.FirstName())
                .RuleFor(a => a.LastName, f => f.Name.LastName())
                .Generate(30);

            context.Actors.AddRange(actors);
            context.SaveChanges();

            // add movies
            var genres = new[] { "Action", "Comedy", "Drama", "Horror", "Sci-Fi", "Romance", "Thriller" };

            var movies = new Faker<Movie>()
                .RuleFor(m => m.Title, f => f.Lorem.Sentence(3).TrimEnd('.'))
                .RuleFor(m => m.Genre, f => f.PickRandom(genres))
                .RuleFor(m => m.Year, f => f.Date.Past(30).Year)
                .RuleFor(m => m.DirectorId, f => f.PickRandom(directors).Id)
                .Generate(20);

            context.Movies.AddRange(movies);
            context.SaveChanges();

            // add actors to movie
            var faker = new Faker();
            var movieActors = new List<MovieActor>();

            foreach (var movie in movies)
            {
                var cast = faker.PickRandom(actors, faker.Random.Int(3, 6)).Distinct();
                foreach (var actor in cast)
                {
                    movieActors.Add(new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actor.Id,
                        Role = faker.Name.JobTitle()
                    });
                }
            }

            context.MovieActors.AddRange(movieActors);
            context.SaveChanges();
        }
    }
}