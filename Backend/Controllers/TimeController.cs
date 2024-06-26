using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Helpers;

namespace Backend.Controllers
{
  [Route("api")]
  [ApiController]
  public class TimeController(PersonaContext context, IStockExchangeService stock, ILogger<TimeController> logger) : ControllerBase
  {
    private readonly PersonaContext _context = context;
    private readonly IStockExchangeService _stock = stock;
    private readonly ILogger<TimeController> _logger = logger;

    [HttpPost("time")]
    public async Task<ActionResult> ReceiveStartSim()
    {
      await _stock.RegisterCompany(10000);
      _logger.LogInformation("Simulation started! Good luck...");
      return Ok();
    } 

    
    [HttpPost("time/reset")]
    public async Task<ActionResult> ReceiveSimReset()
    {
      await _context.RemoveAll();
      _logger.LogInformation("Simulation reset!");
      return Ok();
    }
  }
}
