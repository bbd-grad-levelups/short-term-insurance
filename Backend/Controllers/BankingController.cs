using Backend.Models;
using Backend.Contexts;
using Backend.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Route("api/banking")]
[ApiController]
public class BankingController(PersonaContext context, ISimulationService simulation, ILogger logger) : ControllerBase
{
  private readonly PersonaContext _context = context;
  private readonly ISimulationService _simulation = simulation;
  private readonly ILogger _logger = logger;

  /// <summary>
  /// Endpoint to receive debit order payment success from bank. (Not finalised)
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  [HttpPost("commercial")]
  public async Task<ActionResult> ReceiveCommercialBankNotification([FromBody] BankNotification request)
  {
    // This is now only for receiving payment notificationss
    // Stubbed for now. This check will probably use the debitOrderId in the reference.
    var personaId = request.PersonaId; 
    var currentPersona = await _context.Personas.FirstOrDefaultAsync(p => p.PersonaId == request.PersonaId);
    if (currentPersona != null)
    {
      currentPersona.LastPaymentDate = _simulation.CurrentDate;
      
      await _context.SaveChangesAsync();
    }

    _logger.LogInformation("Received commercial banking payment: {message}", request.Message);
    return Ok();
  }
}
