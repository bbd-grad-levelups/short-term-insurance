using Backend.Contexts;
using Backend.Services;

using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api")]
[ApiController]
public class TimeController(PersonaContext personaContext, LoggerContext loggerContext, IStockExchangeService stock, ITaxService tax, ISimulationService sim, ILogger<TimeController> logger) : ControllerBase
{
  private readonly PersonaContext _personaContext = personaContext;
  private readonly LoggerContext _loggerContext = loggerContext;
  private readonly IStockExchangeService _stock = stock;
  private readonly ITaxService _tax = tax;
  private readonly ISimulationService _simulation = sim;
  private readonly ILogger<TimeController> _logger = logger;

  /// <summary>
  /// Endpoint to receive requests to start the simulation (can be changed a bit to fit with format)
  /// </summary>
  /// <returns></returns>
  [HttpPost("time")]
  public async Task<ActionResult> ReceiveStartSim()
  {
    _logger.LogInformation("Starting simulation! Good luck...");
    _simulation.StartSim();
    
    _logger.LogInformation("Registering Short Term Insurance on the stock exchange.");
    await _stock.Register();
    
    _logger.LogInformation("Registering Short Term Insurance on the revenue service.");
    await _tax.Register();

    return Ok();
  } 

  /// <summary>
  /// Endpoint to receive requests to reset the simulation (can be changed a bit to fit with format)
  /// </summary>
  /// <returns></returns>
  [HttpPost("time/reset")]
  public async Task<ActionResult> ReceiveSimReset()
  {
    await _personaContext.RemoveAll();
    await _loggerContext.RemoveAll();

    _simulation.Reset();
    _logger.LogInformation("Simulation reset!");
    
    return Ok();
  }
}

