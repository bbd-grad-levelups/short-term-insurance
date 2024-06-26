using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Helpers;

namespace TodoApi.Controllers
{
  [Route("api/time")]
  [ApiController]
  public class TimeController(PersonaContext context, IStockExchangeService stock, ILogger<TimeController> logger) : ControllerBase
  {
    private readonly PersonaContext _context = context;
    private readonly IStockExchangeService _stock = stock;
    private readonly ILogger<TimeController> _logger = logger;

    /// <summary>
    /// Endpoint called when a new time update arrives.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("")]
    public async Task<ActionResult<string>> ReceiveTimeUpdate([FromBody] TimeNotification request)
    {
      if (request.CurrentTime == "newMonth")
      {
        // Get monthly profit

        // Send profit to stock exchange
      }

      

      _logger.LogInformation("Current Date: {CurrentTime}", request.CurrentTime);
      return Ok("");
    }

    [HttpPost]
    public async Task<ActionResult> ReceiveStartSim()
    {
      // Send stock notification

      return Ok();
    } 

    
    [HttpGet("reset")]
    public async Task<ActionResult> ReceiveSimReset()
    {
      await context.RemoveAll();
      return Ok();
    }
  }
}
