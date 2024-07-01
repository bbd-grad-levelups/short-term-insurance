using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Helpers;
using Backend.Contexts;

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
  /// <param name="whoEvenKnows">The received information from persona</param>
  /// <returns>Defaults to 204</returns>
  [HttpPost("")]
  public async Task<ActionResult> ReceivePersonaUpdate([FromBody] string whoEvenKnows)
  {
    _logger.LogInformation("Received new persona information");

    List<long> deadPeopleIds = [];
    // Remove dead people (RemovePersona Functionality)
    var deadPeopleAwait = _context.Personas.Where((Persona person) => deadPeopleIds.Contains(person.PersonaId)).ToListAsync();

    // Add next of kin to system
    List<Persona> nextOfKin = [];
    List<Persona> newPeople = nextOfKin.Select(newPerson =>
    {
      return new Persona()
      {
        Electronics = newPerson.Electronics,
        PersonaId = newPerson.PersonaId,
      };
    }).ToList();

    _context.Personas.RemoveRange(await deadPeopleAwait);
    _context.AddRange(newPeople);
    var saveChanges = _context.SaveChangesAsync();

    // Create debit orders for each of these new personas (doesn't have to happen synch)
    newPeople.ForEach(newPerson => 
    {
      var newPremium = Insurance.CalculateInsurance(newPerson.Electronics);
      _banking.CreateRetailDebitOrder(newPerson.PersonaId, newPremium);
    });

    await saveChanges;
    return NoContent();
  }
}