using Backend.Contexts;
using Backend.Services;
using Backend.Models;

using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api")]
[ApiController]
public class TimeController(PersonaContext personaContext, LoggerContext loggerContext, IStockExchangeService stock, ITaxService tax, ISimulationService sim, IPriceService price) : ControllerBase
{
  private readonly PersonaContext _personaContext = personaContext;
  private readonly LoggerContext _loggerContext = loggerContext;
  private readonly IStockExchangeService _stock = stock;
  private readonly IPriceService _price = price;
  private readonly ITaxService _tax = tax;
  private readonly ISimulationService _simulation = sim;

  /// <summary>
  /// Endpoint to receive requests to start the simulation (can be changed a bit to fit with format)
  /// </summary>
  /// <returns></returns>
  [HttpPost("time")]
  public async Task<ActionResult> ReceiveStartSim([FromBody] TimeRequest request)
  {
    List<Log> myLogs = [];
    if (request.Action.Equals("start"))
    {
      _simulation.StartSim(request.StartTime ?? DateTime.Now);

      await _stock.Register();
      await _tax.Register();
      long price = await _price.UpdatePrice();

      myLogs.Add(new Log(_simulation.CurrentDate, $"Starting simulation! Good luck... Time: {DateTime.Now}"));
      myLogs.Add(new Log(_simulation.CurrentDate, $"Received new price for insurance: {price}"));

    }
    else
    {
      _simulation.Reset();
      await _personaContext.RemoveAll();
      await _loggerContext.RemoveAll();

      myLogs.Add(new Log(_simulation.CurrentDate, "Simulation reset!"));
    }

    _loggerContext.AddRange(myLogs);
    await _loggerContext.SaveChangesAsync();

    return Ok();
  } 
}

