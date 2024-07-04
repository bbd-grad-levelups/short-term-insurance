using Backend.Contexts;
using Backend.Models;
using Backend.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Route("api/persona")]
[ApiController]
public class PersonaController(PersonaContext context, LoggerContext logCon, ISimulationService sim, IPriceService price, IBankingService banking, ILogger<PersonaController> logger) : ControllerBase
{
  private readonly PersonaContext _context = context;
  private readonly LoggerContext _logCon = logCon;
  private readonly IBankingService _banking = banking;
  private readonly IPriceService _priceService = price;
  private readonly ISimulationService _simulation = sim;
  private readonly ILogger<PersonaController> _logger = logger;

  /// <summary>
  /// Endpoint called when Persona Manager has new info regarding personas in the system.
  /// </summary>
  /// <param name="personaUpdate">The received information from persona</param>
  /// <returns>Defaults to 204</returns>
  [HttpPost("")]
  public async Task<ActionResult> ReceivePersonaUpdate([FromBody] PersonaUpdate personaUpdate)
  {
    List<Persona> deadPeople = [];
    List<Persona> newPeople = [];
    List<Persona> allPeople = [];
    foreach (Death death in personaUpdate.Deaths)
    {
      var deadPerson = await _context.Personas.Where(persona => persona.PersonaId == death.Deceased).FirstOrDefaultAsync();
      var newPerson = await _context.Personas.Where(persona => persona.PersonaId == death.NextOfKin).FirstOrDefaultAsync();

      if (deadPerson != null)
      {
        deadPeople.Add(deadPerson);
        if (deadPerson.Electronics > 0)
        {
          if (newPerson != null)
          {
            newPerson.Electronics += deadPerson.Electronics;
          }
          else
          {
            newPerson = new Persona()
            {
              Electronics = deadPerson.Electronics,
              PersonaId = death.NextOfKin,
            };
            newPeople.Add(newPerson);
          }
          allPeople.Add(newPerson);
        }
      }
    }

    foreach (Persona persona in allPeople)
    {
      var newPremium = _priceService.CalculateInsurance(persona.Electronics);
      if (persona.DebitOrderId != default)
      {
        await _banking.CancelDebitOrder(persona.DebitOrderId);
      }
      var debitId = await _banking.CreateRetailDebitOrder(persona.PersonaId, newPremium);
      persona.DebitOrderId = debitId;
    }

    _context.Personas.RemoveRange(deadPeople);
    _context.Personas.AddRange(newPeople);
    await _context.SaveChangesAsync();

    var log = new Log(_simulation.CurrentDate, "Received new persona information");
    _logCon.Add(log);
    await _logCon.SaveChangesAsync();
    
    return NoContent();
  }
}