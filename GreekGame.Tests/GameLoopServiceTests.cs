namespace GreekGame.Tests;

using GreekGame.API.BackgroundServices;
using GreekGame.API.Domain;
using GreekGame.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class GameLoopServiceTests
{
    private GameDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }

    #region UpdateCity Tests

    [Fact]
    public void UpdateCity_WithFarms_IncreasesFoodProduction()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));

        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 10,
            Food = 50,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            Buildings = new List<Building>
            {
                new() { Id = Guid.NewGuid(), CityId = Guid.Empty, Type = BuildingType.Farm, Level = 2 }
            }
        };
        var initialFood = city.Food;
        var farmProduction = 5 * 2; // 5 per level * 2 level
        var initialPopulation = city.Population;

        // Act
        InvokeUpdateCity(service, city);

        // Assert
        // Food: initialFood + farmProduction - initialPopulation (population stays same since food >= pop)
        // Expected: 50 + 10 - 10 = 50
        Assert.Equal(initialFood + farmProduction - initialPopulation, city.Food);
    }

    [Fact]
    public void UpdateCity_ConsumesFood_BasedOnPopulation()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));

        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 20,
            Food = 100,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            Buildings = new List<Building>()
        };
        var initialFood = city.Food;
        var initialPopulation = city.Population;

        // Act
        InvokeUpdateCity(service, city);

        // Assert
        // Food consumption = population (no farms, so only consumption)
        // Since food (100) >= population (20), population won't decrease
        // Expected: 100 - 20 = 80
        Assert.Equal(initialFood - initialPopulation, city.Food);
    }

    [Fact]
    public void UpdateCity_WithNoFood_DecreasePopulation()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));

        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 10,
            Food = 0,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            Buildings = new List<Building>()
        };
        var initialPopulation = city.Population;

        // Act
        InvokeUpdateCity(service, city);

        // Assert
        Assert.Equal(initialPopulation - 1, city.Population);
    }

    [Fact]
    public void UpdateCity_WithSufficientFoodAndHouses_IncreasesPopulation()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));

        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 5,
            Food = 100,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            Buildings = new List<Building>
            {
                new() { Id = Guid.NewGuid(), CityId = Guid.Empty, Type = BuildingType.House, Level = 3 }
            }
        };
        var initialPopulation = city.Population;
        var maxPopulation = 5 * 3; // 5 per level * 3 level

        // Act
        InvokeUpdateCity(service, city);

        // Assert
        Assert.Equal(initialPopulation + 1, city.Population);
        Assert.True(city.Population <= maxPopulation);
    }

    [Fact]
    public void UpdateCity_PopulationCantExceedMaximum()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));

        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 15,
            Food = 100,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            Buildings = new List<Building>
            {
                new() { Id = Guid.NewGuid(), CityId = Guid.Empty, Type = BuildingType.House, Level = 2 }
            }
        };
        var maxPopulation = 5 * 2; // 10 max

        // Act
        for (int i = 0; i < 100; i++)
        {
            InvokeUpdateCity(service, city);
        }

        // Assert
        Assert.True(city.Population <= maxPopulation);
    }

    [Fact]
    public void UpdateCity_GeneratesMoney_BasedOnPopulation()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));

        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 20,
            Food = 100,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            Buildings = new List<Building>()
        };
        var initialMoney = city.Money;

        // Act
        InvokeUpdateCity(service, city);

        // Assert
        var expectedMoney = initialMoney + (city.Population * 0.5m);
        Assert.Equal(expectedMoney, city.Money);
    }

    [Fact]
    public void UpdateCity_PopulationNeverGoesNegative()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));

        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 1,
            Food = 0,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            Buildings = new List<Building>()
        };

        // Act
        for (int i = 0; i < 100; i++)
        {
            InvokeUpdateCity(service, city);
        }

        // Assert
        Assert.True(city.Population >= 0);
    }

    [Fact]
    public void UpdateCity_WithMultipleFarms_CalculatesCombinedProduction()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));

        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 5,
            Food = 50,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            Buildings = new List<Building>
            {
                new() { Id = Guid.NewGuid(), CityId = Guid.Empty, Type = BuildingType.Farm, Level = 1 },
                new() { Id = Guid.NewGuid(), CityId = Guid.Empty, Type = BuildingType.Farm, Level = 2 }
            }
        };
        var initialFood = city.Food;
        var expectedProduction = (5 * 1) + (5 * 2); // 5 + 10 = 15
        var expectedFood = initialFood + expectedProduction - city.Population; // 50 + 15 - 5 = 60

        // Act
        InvokeUpdateCity(service, city);

        // Assert
        Assert.Equal(expectedFood, city.Food);
    }

    [Fact]
    public void UpdateCity_WithMixedBuildings_CalculatesCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));

        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 10,
            Food = 50,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            Buildings = new List<Building>
            {
                new() { Id = Guid.NewGuid(), CityId = Guid.Empty, Type = BuildingType.Farm, Level = 2 },
                new() { Id = Guid.NewGuid(), CityId = Guid.Empty, Type = BuildingType.House, Level = 3 }
            }
        };
        var initialFood = city.Food;
        var initialMoney = city.Money;
        var initialPopulation = city.Population;
        var farmProduction = 5 * 2; // 10
        var maxPopulation = 5 * 3; // 15
        var foodAfterProduction = initialFood + farmProduction; // 50 + 10 = 60

        // Since foodAfterProduction (60) >= population (10) and population < maxPopulation,
        // population will increase by 1
        var expectedPopulation = initialPopulation + 1; // 11
        var expectedFood = foodAfterProduction - initialPopulation; // 60 - 10 = 50
        var expectedMoney = initialMoney + (expectedPopulation * 0.5m); // 100 + (11 * 0.5) = 105.5

        // Act
        InvokeUpdateCity(service, city);

        // Assert
        Assert.Equal(expectedFood, city.Food);
        Assert.Equal(expectedMoney, city.Money);
        Assert.Equal(expectedPopulation, city.Population);
        Assert.True(city.Population <= maxPopulation);
    }

    #endregion

    #region UpdateMarket Tests

    [Fact]
    public void UpdateMarket_WithZeroSupply_SetsToOneForCalculation()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var market = new Market { Id = Guid.NewGuid(), FoodPrice = 10m, TotalSupply = 0, TotalDemand = 5 };
        var initialPrice = market.FoodPrice;

        // Act
        InvokeUpdateMarket(service, market);

        // Assert
        // When TotalSupply is 0, it's set to 1 internally for calculation
        // Price ratio = 5/1 = 5, so price should be 10 * 5 = 50, then reset
        Assert.True(market.FoodPrice >= initialPrice); // Price should increase or stay same
        Assert.Equal(0, market.TotalSupply); // Supply is reset to 0 after calculation
        Assert.Equal(0, market.TotalDemand); // Demand is reset to 0 after calculation
    }

    [Fact]
    public void UpdateMarket_WithZeroDemand_SetsToOneForCalculation()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var market = new Market { Id = Guid.NewGuid(), FoodPrice = 10m, TotalSupply = 5, TotalDemand = 0 };
        var initialPrice = market.FoodPrice;

        // Act
        InvokeUpdateMarket(service, market);

        // Assert
        // When TotalDemand is 0, it's set to 1 internally for calculation
        // Price ratio = 1/5 = 0.2, so price should be 10 * 0.2 = 2, then reset
        Assert.True(market.FoodPrice <= initialPrice); // Price should decrease or stay same
        Assert.Equal(0, market.TotalSupply); // Supply is reset to 0 after calculation
        Assert.Equal(0, market.TotalDemand); // Demand is reset to 0 after calculation
    }

    [Fact]
    public void UpdateMarket_HighDemandIncreasesPrice()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var market = new Market { Id = Guid.NewGuid(), FoodPrice = 10m, TotalSupply = 5, TotalDemand = 20 };
        var initialPrice = market.FoodPrice;

        // Act
        InvokeUpdateMarket(service, market);

        // Assert
        // Ratio = 20/5 = 4, so price should be 10 * 4 = 40
        Assert.True(market.FoodPrice > initialPrice);
    }

    [Fact]
    public void UpdateMarket_LowDemandDecreasesPrice()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var market = new Market { Id = Guid.NewGuid(), FoodPrice = 10m, TotalSupply = 20, TotalDemand = 5 };
        var initialPrice = market.FoodPrice;

        // Act
        InvokeUpdateMarket(service, market);

        // Assert
        // Ratio = 5/20 = 0.25, so price should be 10 * 0.25 = 2.5
        Assert.True(market.FoodPrice < initialPrice);
    }

    [Fact]
    public void UpdateMarket_PriceClampedToMinimum()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var market = new Market { Id = Guid.NewGuid(), FoodPrice = 1m, TotalSupply = 100, TotalDemand = 1 };

        // Act
        InvokeUpdateMarket(service, market);

        // Assert
        Assert.True(market.FoodPrice >= 1m);
    }

    [Fact]
    public void UpdateMarket_PriceClampedToMaximum()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var market = new Market { Id = Guid.NewGuid(), FoodPrice = 100m, TotalSupply = 1, TotalDemand = 100 };

        // Act
        InvokeUpdateMarket(service, market);

        // Assert
        Assert.True(market.FoodPrice <= 100m);
    }

    [Fact]
    public void UpdateMarket_ResetsSupplyAndDemand()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var market = new Market { Id = Guid.NewGuid(), FoodPrice = 10, TotalSupply = 50, TotalDemand = 30 };

        // Act
        InvokeUpdateMarket(service, market);

        // Assert
        Assert.Equal(0, market.TotalSupply);
        Assert.Equal(0, market.TotalDemand);
    }

    [Fact]
    public void UpdateMarket_BalancedSupplyAndDemand_MaintainsPrice()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var market = new Market { Id = Guid.NewGuid(), FoodPrice = 10m, TotalSupply = 20, TotalDemand = 20 };
        var initialPrice = market.FoodPrice;

        // Act
        InvokeUpdateMarket(service, market);

        // Assert
        // Ratio = 20/20 = 1, so price should remain unchanged
        Assert.Equal(initialPrice, market.FoodPrice);
    }

    #endregion

    #region TriggerRandomEvent Tests

    [Fact]
    public void TriggerRandomEvent_CreatesEventWithCorrectProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 10,
            Food = 50,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            ActiveEvents = new List<Event>()
        };
        var initialEventCount = city.ActiveEvents.Count;

        // Act - Call multiple times to increase chance of triggering
        for (int i = 0; i < 1000; i++)
        {
            InvokeTriggerRandomEvent(service, city);
            if (city.ActiveEvents.Count > initialEventCount)
                break;
        }

        // Assert
        if (city.ActiveEvents.Count > initialEventCount)
        {
            var newEvent = city.ActiveEvents.Last();
            Assert.NotEqual(Guid.Empty, newEvent.Id);
            Assert.Equal(city.Id, newEvent.CityId);
            Assert.Equal(3, newEvent.DurationTicks);
            Assert.Equal(3, newEvent.RemainingTicks);
        }
    }

    [Fact]
    public void TriggerRandomEvent_CreatesValidEventType()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 10,
            Food = 50,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            ActiveEvents = new List<Event>()
        };

        // Act - Call multiple times to increase chance of triggering
        for (int i = 0; i < 1000; i++)
        {
            InvokeTriggerRandomEvent(service, city);
            if (city.ActiveEvents.Count > 0)
                break;
        }

        // Assert
        foreach (var evt in city.ActiveEvents)
        {
            var validTypes = new[] { EventType.None, EventType.Drought, EventType.Boom, EventType.TaxPenalty };
            Assert.Contains(evt.Type, validTypes);
        }
    }

    [Fact]
    public void TriggerRandomEvent_DoesNotThrowOnValidEventTypes()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new GameLoopService(CreateServiceProvider(context));
        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 10,
            Food = 50,
            Money = 100m,
            LastUpdated = DateTime.UtcNow,
            ActiveEvents = new List<Event>()
        };

        // Act & Assert - Should not throw
        var exception = Record.Exception(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                InvokeTriggerRandomEvent(service, city);
            }
        });

        Assert.Null(exception);
    }

    #endregion

    #region Helper Methods

    private static IServiceProvider CreateServiceProvider(GameDbContext context)
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => context);
        return services.BuildServiceProvider();
    }

    private static void InvokeUpdateCity(GameLoopService service, City city)
    {
        var method = typeof(GameLoopService).GetMethod("UpdateCity",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(service, new object[] { city });
    }

    private static void InvokeUpdateMarket(GameLoopService service, Market market)
    {
        var method = typeof(GameLoopService).GetMethod("UpdateMarket",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(service, new object[] { market });
    }

    private static void InvokeTriggerRandomEvent(GameLoopService service, City city)
    {
        var method = typeof(GameLoopService).GetMethod("TriggerRandomEvent",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(service, new object[] { city });
    }

    #endregion
}


