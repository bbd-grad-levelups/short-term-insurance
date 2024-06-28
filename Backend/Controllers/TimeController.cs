using Microsoft.AspNetCore.Mvc;
using Backend.Helpers;
using Backend.Contexts;

namespace Backend.Controllers;

[Route("api")]
[ApiController]
public class TimeController(PersonaContext context, IStockExchangeService stock, ISimulationService sim, ILogger<TimeController> logger) : ControllerBase
{
  private readonly PersonaContext _context = context;
  private readonly IStockExchangeService _stock = stock;
  private readonly ISimulationService _simulation = sim;
  private readonly ILogger<TimeController> _logger = logger;

  /// <summary>
  /// Endpoint to receive requests to start the simulation (can be changed a bit to fit with format)
  /// </summary>
  /// <returns></returns>
  [HttpPost("time")]
  public async Task<ActionResult> ReceiveStartSim()
  {
    _simulation.StartSim();
    await _stock.RegisterCompany();
    _logger.LogInformation("Simulation started! Good luck...");
    return Ok();
  } 

  /// <summary>
  /// Endpoint to receive requests to reset the simulation (can be changed a bit to fit with format)
  /// </summary>
  /// <returns></returns>
  [HttpPost("time/reset")]
  public async Task<ActionResult> ReceiveSimReset()
  {
    await _context.RemoveAll();
    _logger.LogInformation("Simulation reset!");
    return Ok();
  }
}

