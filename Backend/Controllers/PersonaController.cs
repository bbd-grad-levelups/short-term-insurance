using Backend.Contexts;
using Backend.Models;
using Backend.Helpers;
using Backend.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Route("api/persona")]
[ApiController]
public class PersonaController(PersonaContext context, IBankingService banking, ILogger<PersonaController> logger) : ControllerBase
{
  private readonly PersonaContext _context = context;
  private readonly IBankingService _banking = banking;
  private readonly ILogger<PersonaController> _logger = logger;

  /// <summary>
  /// Endpoint called when Persona Manager has new info regarding personas in the system.
  /// </summary>
  /// <param name="personaUpdate">The received information from persona</param>
  /// <returns>Defaults to 204</returns>
  [HttpPost("")]
  public async Task<ActionResult> ReceivePersonaUpdate([FromBody] PersonaUpdate personaUpdate)
  {
    _logger.LogInformation("Received new persona information");

    List<long> deadPeopleIds = personaUpdate.Deaths.Select(death => death.Deceased).ToList();
    var deadPeople = await _context.Personas.Where((Persona person) => deadPeopleIds.Contains(person.PersonaId)).ToListAsync();

    List<Persona> newPeople = personaUpdate.Deaths.Select(currentDeath =>
    {
      var deadPerson = deadPeople.FirstOrDefault(persona => persona.PersonaId == currentDeath.Deceased);
      return new Persona()
      {
        Electronics = deadPerson?.Electronics ?? 0,
        PersonaId = currentDeath.NextOfKin,
      };
    }).ToList();

    _context.Personas.RemoveRange(deadPeople);
    
    // Create debit orders for each of these new personas
    var additionsTasks = newPeople.Select(async newPerson => 
    {
      var newPremium = Insurance.CalculateInsurance(newPerson.Electronics);
      var debitId = await _banking.CreateRetailDebitOrder(newPerson.PersonaId, newPremium);
      newPerson.DebitOrderId = debitId;
      return newPerson;
    }).ToList();

    var additions = Task.WhenAll(additionsTasks);

    _context.AddRange(newPeople);
    await _context.SaveChangesAsync();
    return NoContent();
  }
}