using Backend.Contexts;
using Backend.Models;
using Backend.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Route("api/electronics")]
[ApiController]
public class ElectronicsController(PersonaContext context, LoggerContext logCon, IBankingService banking, ISimulationService simulation, IPriceService price, ILogger<ElectronicsController> logger) : ControllerBase
{
  private readonly PersonaContext _context = context;
  private readonly LoggerContext loggerContext = logCon;
  private readonly IBankingService _banking = banking;
  private readonly IPriceService _priceService = price;
  private readonly ISimulationService _simulation = simulation;
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
      Log myLog;
      var persona = await _context.Personas.FirstOrDefaultAsync(p => p.PersonaId == request.PersonaId);

      if (persona != null)
      {
        int electronicsClaimed = Math.Min(request.AmountDestroyed, persona.Electronics);
        bool currentlyInsured = _simulation.DaysSinceDate(persona.LastPaymentDate) < 32;
        if (currentlyInsured)
        {
          long claimPayout = _priceService.CalculatePayout(electronicsClaimed);
          await _banking.MakeCommercialPayment(persona.PersonaId.ToString(), "Electronics insurance payout.", claimPayout);
        }

        persona.Electronics -= electronicsClaimed;

        var newPremium = _priceService.CalculateInsurance(persona.Electronics);
        await _banking.CancelDebitOrder(persona.DebitOrderId);
        int newDebitId = await _banking.CreateRetailDebitOrder(persona.PersonaId, newPremium);
        persona.DebitOrderId = newDebitId;
        await _context.SaveChangesAsync();
        
        myLog = new Log(_simulation.CurrentDate, $"Received claim for {persona.PersonaId}, paid out: {currentlyInsured}");
      }
      else
      {
        _logger.LogInformation("Received claim for persona {personaId} that doesn't have insurance", request.PersonaId);
        myLog = new Log(_simulation.CurrentDate, $"Received claim for persona {request.PersonaId} that doesn't have insurance");
      }

      loggerContext.Add(myLog);
      await loggerContext.SaveChangesAsync();
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
      var currentPersona = await _context.Personas.Where(p => p.PersonaId == request.PersonaId).FirstOrDefaultAsync();
      if (currentPersona != null)
      {
        currentPersona.Electronics += request.AmountNew;
        await _banking.CancelDebitOrder(currentPersona.DebitOrderId);
      }
      else
      {
        currentPersona ??= new Persona()
        {
          PersonaId = request.PersonaId,
          Electronics = request.AmountNew,
        };
        _context.Add(currentPersona);
      }

      var newPremium = _priceService.CalculateInsurance(currentPersona.Electronics);
      int newDebitId = await _banking.CreateRetailDebitOrder(currentPersona.PersonaId, newPremium);
      currentPersona.DebitOrderId = newDebitId;
      await _context.SaveChangesAsync();

      var myLog = new Log(_simulation.CurrentDate, $"Received new insurance request ({request.AmountNew} electronics) for {currentPersona.PersonaId}");
      loggerContext.Add(myLog);
      await loggerContext.SaveChangesAsync();
    }

    return NoContent();
  }
}