using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Helpers;

namespace TodoApi.Controllers
{
  [Route("api/banking")]
  [ApiController]
  public class BankingController(PersonaContext context, IBankingService banking, ILogger logger) : ControllerBase
  {
    private readonly PersonaContext _context = context;
    private readonly IBankingService _banking = banking;
    private readonly ILogger _logger = logger;

    // Currently stubbed, pending bank API spec
    /// <summary>
    /// Endpoint to receive debit order payment success from bank.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("commercial")]
    public async Task<ActionResult<string>> ReceiveCommercialBankNotification([FromBody] BankNotification request)
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

    // Response on request for an updated debit order
    /// <summary>
    /// Endpoint to receive response on updating a debit order.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("retail")]
    public async Task<ActionResult> ReceiveRetailBankNotification([FromBody] BankNotification request)
    {
      if (request.Message == "Debit update")
      {
        if (!request.Success)
        {
          // Might need to retry the update, pending what messages they actually have
        }
      }
      _logger.LogInformation("Retail notification: " + request.ToString());

      return Ok();
    }

    // Response on tax sending request for tax
    /// <summary>
    /// Endpoint for getting request to pay tax or something, who fucking knows
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("tax")]
    public async Task<ActionResult<string>> ReceiveTaxPaymentRequest([FromBody] TaxNotification request)
    {
      // Bank payment, probably not using this endpoint tbh
      return Ok("");
    }
  }
}
