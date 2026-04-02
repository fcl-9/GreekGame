namespace GreekGame.API.Application;

using GreekGame.API.Domain;

public static class BuildingUpgrades
{
    public static decimal GetBuildingCost(BuildingType type)
    {
        return type switch
        {
            BuildingType.Farm => 50m,
            BuildingType.House => 100m,
            _ => 0m
        };
    }

    public static decimal GetUpgradeCost(BuildingType type, int buildingLevel)
    {
        return type switch
        {
            BuildingType.Farm => 50m * buildingLevel,
            BuildingType.House => 100m * buildingLevel,
            _ => 0m
        };
    }
}
