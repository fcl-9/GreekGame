using GreekGame.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GreekGame.API.Infrastructure;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }

    public DbSet<City> Cities => Set<City>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Market> Markets => Set<Market>();
}
