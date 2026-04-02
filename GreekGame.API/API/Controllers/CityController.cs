namespace GreekGame.API.API.Controllers;

using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        this._cityService = cityService;
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var city = await _cityService.CreateCityAsync();
        return Ok(city);
    }

    [HttpPost("{id}/build")]
    public async Task<IActionResult> CreateBuilding(Guid id, [FromBody] BuildingType buildingType)
    {
        var success = await _cityService.BuildBuildingAsync(id, buildingType);
        return success ? Ok() : BadRequest();
    }

    [HttpPost("buildings/{id}/upgrade")]
    public async Task<IActionResult> CreateBuilding(Guid id)
    {
        var success = await _cityService.UpgradeBuildingAsync(id);
        return success ? Ok() : BadRequest();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCity(Guid id)
    {
        var city = await _cityService.GetCityAsync(id);
        return city is null ? NotFound() : Ok(city);
    }

    [HttpGet("{id}/events")]
    public async Task<IActionResult> GetEvents(Guid id)
    {
        var city = await _cityService.GetCityAsync(id);
        if (city is null)
            return NotFound();
        var activeEvents = city.ActiveEvents;

        return activeEvents.Count <= 0 ? NotFound() : Ok(city.ActiveEvents);
    }
}
