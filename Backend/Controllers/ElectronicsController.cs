using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Helpers;

namespace Backend.Controllers;

[Route("api/electronics")]
[ApiController]
public class ElectronicsController(PersonaContext context, IBankingService banking, ILogger<ElectronicsController> logger) : ControllerBase
{
  private readonly PersonaContext _context = context;
  private readonly IBankingService _banking = banking;
  private readonly ILogger<ElectronicsController> _logger = logger;

  /// <summary>
  /// Endpoint called when electronics get destroyed. Performs a claim payout and assigns a new debit order.
  /// </summary>
  /// <param name="request">The request body</param>
  /// <returns>Nothing, it's a queue</returns>
  [HttpPatch("")]
  public async Task<ActionResult> RemoveElectronics([FromBody] DestroyedElectronics request)
  {
    if (request.AmountDestroyed > 0)
    {
      var persona = await _context.Personas.FirstOrDefaultAsync(p => p.PersonaId == request.PersonaId);

      if (persona != null)
      {
        _logger.LogInformation("Found persona: {ToString}", persona.ToString());
        bool boundedClaim = request.AmountDestroyed <= persona.Electronics;

        int electronicsClaimed = boundedClaim ? request.AmountDestroyed : persona.Electronics;
        if (!persona.Blacklisted)
        {
          double claimPayout = Insurance.CalculatePayout(electronicsClaimed);
          await _banking.MakeCommercialPayment(persona.PersonaId, claimPayout);
        }

        persona.Electronics = boundedClaim ? persona.Electronics - request.AmountDestroyed : 0;
        _context.Entry(persona).State = EntityState.Modified;

        var newPremium = Insurance.CalculateInsurance(persona.Electronics);
        await _context.SaveChangesAsync();
        await _banking.CreateRetailDebitOrder(persona.PersonaId, newPremium);
      }
      else
      {
        _logger.LogInformation("Electronics destroyed for persona '{PersonaId}', but they are not on the system", request.PersonaId);
      }
    }

    return NoContent();
  }

  // This one is when person's electronics amount changes.
  // Tracks new gained (can be negative?), and destroyed
  /// <summary>
  /// Endpoint called when someone has bought new electronics which need to be insured.
  /// </summary>
  /// <param name="request">Request body</param>
  /// <returns></returns>
  [HttpPut("")]
  public async Task<ActionResult<double>> AddElectronics([FromBody] AddElectronics request)
  {
    if (request.AmountNew > 0)
    {
      var currentPersona = await _context.Personas.FirstAsync(p => p.PersonaId == request.PersonaId);
      // Create person if they didn't exist before
      if (currentPersona != null)
      {
        currentPersona.Electronics += request.AmountNew;
      }
      else
      {
        currentPersona ??= new Persona()
        {
          Blacklisted = false,
          PersonaId = request.PersonaId,
          Electronics = request.AmountNew
        };
        _context.Add(currentPersona);
      }
      var saveAwait = _context.SaveChangesAsync();

      var newPremium = Insurance.CalculateInsurance(currentPersona.Electronics);

      await _banking.CreateRetailDebitOrder(currentPersona.PersonaId, newPremium);
      await saveAwait;
    }

    return NoContent();
  }

  /// <summary>
  /// Endpoint called when price of insurance changes.
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  [HttpPatch("price")]
  public ActionResult UpdatePrice([FromBody] ModifyInsurancePrice request)
  {
    Insurance.CurrentPrice = request.NewPrice;
    return NoContent();
  }
}