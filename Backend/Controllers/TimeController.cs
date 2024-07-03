using Backend.Contexts;
using Backend.Services;
using Backend.Models;

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
  public async Task<ActionResult> ReceiveStartSim([FromBody] TimeRequest request)
  {
    Console.WriteLine("Sim start console log");
    if (request.Action.Equals("start"))
    {
      _logger.LogInformation("Starting simulation! Good luck...");
      _simulation.StartSim(request.StartTime ?? DateTime.Now);

      _logger.LogInformation("Registering Short Term Insurance on the stock exchange.");
      await _stock.Register();

      _logger.LogInformation("Registering Short Term Insurance on the revenue service.");
      await _tax.Register();
    }
    else
    {
      _simulation.Reset();
      await _personaContext.RemoveAll();
      await _loggerContext.RemoveAll();
      _logger.LogInformation("Simulation reset!");
    }
    
    return Ok();
  } 
}

