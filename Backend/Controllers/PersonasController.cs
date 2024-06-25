using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Helpers;

namespace TodoApi.Controllers
{
  [Route("api")]
  [ApiController]
  public class PersonasController(PersonaContext context) : ControllerBase
  {
    private readonly PersonaContext _context = context;

    /// <summary>
    /// Retrieves a list of personas, paginated.
    /// Admin frontend only, not useful to other services.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a paginated list of personas.
    /// </remarks>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>A list of personas</returns>
    [HttpGet("personas")]
    public async Task<ActionResult<IEnumerable<Persona>>> GetPersonas(int page = 1, int pageSize = 50)
    {
      var personas = await _context.Personas
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

      if (personas.Count == 0)
      {
        return NotFound();
      }

      return Ok(personas);
    }

    // Stubbed for now, possible logging endpoint for frontend
    [HttpGet("log")]
    public async Task<ActionResult<string>> GetLog(int page = 1, int pageSize = 50)
    {
      return Ok("Whoop");
    }

    // Post electronics when someone wants to register for insurance. They need to give their bank account?
    [HttpPost("electronics")]
    public async Task<ActionResult<Persona>> NewElectronics([FromBody] RegisterElectronics request)
    {
      var persona = _context.Personas.Where(p => p.PersonaId == request.PersonaId).First();

      if (persona == null)
      {
        var newPersona = new Persona()
        {
          Blacklisted = true,
          Electronics = request.ElectronicsAmount,
          PersonaId = request.PersonaId,
          BankAccount = request.BankAccount
        };

        await _context.AddAsync(newPersona);

        double newDebit = Insurance.CalculateInsurance(newPersona.Electronics);
        Banking.UpdateRetailDebitOrder(request.BankAccount, newDebit);

        return Ok();
      }
      else
      {
        return BadRequest("Persona is already registered for insurance");
      }
    }

    // Person has died
    [HttpDelete("electronics")]
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
    [HttpPut("electronics")]
    public async Task<ActionResult<double>> PutPersona([FromBody] ModifyElectronics request)
    {
      var currentPersona = await _context.Personas.FirstOrDefaultAsync(p => p.PersonaId == request.PersonaId);

      if (currentPersona == null)
      {
        return NotFound();
      }
      else
      {
        int totalChanged = request.AmountNew - request.AmountDestroyed;
        double newDebitOrder = 1;

        if (request.AmountDestroyed > 0)
        {
          if (!currentPersona.Blacklisted)
          {
            // Tell bank to pay out cash
          }
        }

        if (totalChanged != 0)
        {
          // Update debit order
        }

        currentPersona.Electronics += totalChanged;
        _context.Entry(currentPersona).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return Ok(newDebitOrder);
      }
    }

    [HttpPatch("electronics/price")]
    public async Task<ActionResult> UpdatePrice([FromBody] ModifyElectronicsPrice request)
    {
      Insurance.CurrentPrice = request.ElectronicsPrice;
      return Ok();
    }

  }
}
