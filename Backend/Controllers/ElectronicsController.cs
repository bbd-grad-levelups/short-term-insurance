using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Helpers;

namespace TodoApi.Controllers;

[Route("api/electronics")]
[ApiController]
public class ElectronicsController(PersonaContext context, IBankingService banking) : ControllerBase
{
  private readonly PersonaContext _context = context;
  private readonly IBankingService _banking = banking;

  /// <summary>
  /// Endpoint called when electronics get destroyed. Performs a claim payout and assigns a new debit order.
  /// </summary>
  /// <param name="request">The request body</param>
  /// <returns>Nothing, it's a queue</returns>
  [HttpPost("")]
  public async Task<ActionResult<Persona>> RemoveElectronics([FromBody] DestroyedElectronics request)
  {
    if (request.AmountDestroyed > 0)
    {
      var persona = _context.Personas.Where(p => p.PersonaId == request.PersonaId).First();

      bool boundedClaim = request.AmountDestroyed <= persona.Electronics;
      if (persona != null)
      {
        int electronicsClaimed = boundedClaim ? request.AmountDestroyed : persona.Electronics;
        if (!persona.Blacklisted)
        {
          double claimPayout = Insurance.CalculatePayout(electronicsClaimed);
          await _banking.MakeCommercialPayment(persona.PersonaId, claimPayout).ConfigureAwait(false);
        }

        persona.Electronics = boundedClaim ? persona.Electronics - request.AmountDestroyed : 0;
        _context.Entry(persona).State = EntityState.Modified;

        var newPremium = Insurance.CalculateInsurance(persona.Electronics);
        await _context.SaveChangesAsync();
        await _banking.CreateRetailDebitOrder(persona.PersonaId, newPremium).ConfigureAwait(false);
      }
    }

    return Ok();
  }

  /// <summary>
  /// Endpoint called when someone has died, and they will no longer be needing insurance for their stuff.
  /// </summary>
  /// <param name="request">Request body</param>
  /// <returns>Nothing, it's a queue</returns>
  [HttpDelete("")]
  public async Task<ActionResult<string>> RemovePersona([FromBody] DeregisterElectronics request)
  {
    // No need to change anything external, bank account closes and we get a notification if new person wants insurance
    var personaToDelete = await _context.Personas.FirstOrDefaultAsync(p => p.PersonaId == request.PersonaId);

    if (personaToDelete == null)
    {
      return NotFound();
    }
    else
    {
      _context.Personas.Remove(personaToDelete);
      await _context.SaveChangesAsync();
      return Ok();
    }
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
      currentPersona ??= new Persona()
      {
        Blacklisted = false,
        PersonaId = request.PersonaId,
        Electronics = 0
      };

      currentPersona.Electronics += request.AmountNew;
      _context.Entry(currentPersona).State = EntityState.Modified;

      var newPremium = Insurance.CalculateInsurance(currentPersona.Electronics);
      await _context.SaveChangesAsync();

      await _banking.CreateRetailDebitOrder(currentPersona.PersonaId, newPremium).ConfigureAwait(false);
    }

    return Ok();
  }

  /// <summary>
  /// Endpoint called when price of insurance changes.
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  [HttpPatch("price")]
  public async Task<ActionResult> UpdatePrice([FromBody] ModifyInsurancePrice request)
  {
    Insurance.CurrentPrice = request.NewPrice;
    return Ok();
  }
}