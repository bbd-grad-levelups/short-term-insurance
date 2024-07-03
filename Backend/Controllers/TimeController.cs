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
    Log myLog;
    if (request.Action.Equals("start"))
    {
      _simulation.StartSim(request.StartTime ?? DateTime.Now);

      await _stock.Register();
      await _tax.Register();

      myLog = new Log(_simulation.CurrentDate, $"Starting simulation! Good luck... Time: {DateTime.Now}");
      
    }
    else
    {
      _simulation.Reset();
      await _personaContext.RemoveAll();
      await _loggerContext.RemoveAll();

      myLog = new Log(_simulation.CurrentDate, "Simulation reset!");
    }

    _loggerContext.Add(myLog);
    await _loggerContext.SaveChangesAsync();

    return Ok();
  } 
}

