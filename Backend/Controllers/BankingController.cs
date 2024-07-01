using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Helpers;
using Backend.Contexts;

namespace Backend.Controllers;

[Route("api/banking")]
[ApiController]
public class BankingController(PersonaContext context, IBankingService banking, ILogger logger) : ControllerBase
{
  private readonly PersonaContext _context = context;
  private readonly IBankingService _banking = banking;
  private readonly ILogger _logger = logger;

  // Currently stubbed, pending bank API spec
  /// <summary>
  /// Endpoint to receive debit order payment success from bank. (Not finalised)
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  [HttpPost("commercial")]
  public async Task<ActionResult> ReceiveCommercialBankNotification([FromBody] BankNotification request)
  {
    // This will basically just be "we failed payment" or "persona failed payment"
    if (request.Message == "Persona payment")
    {
      var currentPersona = await _context.Personas.FirstOrDefaultAsync(p => p.PersonaId == request.PersonaId);

      if (currentPersona == null)
      {
        return Ok();
      }
      else
      {
        currentPersona.Blacklisted = request.Success;

        _context.Entry(currentPersona).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return Ok();
      }
    }
    else if (request.Message == "Company payment")
    {
      if (!request.Success)
      {
        // Either retry payment, or take out a loan before retrying?
      }

      return Ok();
    }
    else return Ok();
  }
}
