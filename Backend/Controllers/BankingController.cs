using Backend.Models;
using Backend.Contexts;
using Backend.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Route("api/banking")]
[ApiController]
public class BankingController(PersonaContext context, ISimulationService simulation, ITaxService tax, ILogger logger) : ControllerBase
{
  private readonly PersonaContext _context = context;
  private readonly ISimulationService _simulation = simulation;
  private readonly ITaxService _taxService = tax;
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
    if (request.Type.Equals("incoming_payment"))
    {
      var success = int.TryParse(request.Transaction.Reference, out int debitOrderId);

      var currentPersona = await _context.Personas.FirstOrDefaultAsync(p => p.DebitOrderId == debitOrderId);
      if (currentPersona != null)
      {
        currentPersona.LastPaymentDate = _simulation.CurrentDate;

        await _context.SaveChangesAsync();
      }

      _taxService.Profit += (int)Math.Round(request.Transaction.Amount);
    }
    else if (request.Type.Equals("outgoing_payment"))
    {
      _taxService.Profit -= (int)Math.Round(request.Transaction.Amount);
    }

    

    _logger.LogInformation("Received commercial banking payment: {message}", request.Transaction.ToString());
    return Ok();
  }
}
