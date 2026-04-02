namespace GreekGame.API.Infrastructure;

using GreekGame.API.Domain;
using Microsoft.EntityFrameworkCore;

public class GameDbContext : DbContext
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Market> Markets => Set<Market>();

    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }
}
