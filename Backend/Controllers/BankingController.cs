using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace TodoApi.Controllers
{
  [Route("api")]
  [ApiController]
  public class BankingController(PersonaContext context, string banking) : ControllerBase
  {
    private readonly PersonaContext _context = context;
    private readonly string _banking = banking;

    // Currently stubbed, pending bank API spec
    [HttpPut("banking/commercial")]
    public async Task<ActionResult<string>> ReceiveCommercialBankNotification([FromBody] BankNotification request)
    {
      // This will basically just be "we failed payment" or "persona failed payment"
      if (request.Message == "Persona payment")
      {
        var currentPersona = await _context.Personas.FirstOrDefaultAsync(p => p.PersonaId == request.PersonaId);

        if (currentPersona == null)
        {
          // person doesn't exist, whatever
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
    [HttpPost("banking/retail")]
    public async Task<ActionResult> ReceiveRetailBankNotification([FromBody] BankNotification request)
    {
      if (request.Message == "Debit update")
      {
        if (!request.Success)
        {
          // Might need to retry the update, pending what messages they actually have
        }
      }

      return Ok();
    }

    // Response on tax sending request for tax
    [HttpPut("tax")]
    public async Task<ActionResult<string>> ReceiveTaxPaymentRequest([FromBody] TaxNotification request)
    {
      // Bank payment, probably not using this endpoint tbh
      return Ok("");
    }
  }
}
