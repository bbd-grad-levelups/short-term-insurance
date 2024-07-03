using Backend.Contexts;
using Backend.Helpers;
using Backend.Models;
using Backend.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Route("api/electronics")]
[ApiController]
public class ElectronicsController(PersonaContext context, LoggerContext logCon, IBankingService banking, ISimulationService simulation, ILogger<ElectronicsController> logger) : ControllerBase
{
  private readonly PersonaContext _context = context;
  private readonly IBankingService _banking = banking;
  private readonly LoggerContext loggerContext = logCon;
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
      var persona = await _context.Personas.FirstOrDefaultAsync(p => p.PersonaId == request.PersonaId);

      if (persona != null)
      {
        int electronicsClaimed = Math.Min(request.AmountDestroyed, persona.Electronics);
        bool currentlyInsured = _simulation.DaysSinceDate(persona.LastPaymentDate) < 32;
        if (currentlyInsured)
        {
          long claimPayout = Insurance.CalculatePayout(electronicsClaimed);
          await _banking.MakeCommercialPayment(persona.PersonaId, claimPayout);
        }

        persona.Electronics -= electronicsClaimed;

        var newPremium = Insurance.CalculateInsurance(persona.Electronics);
        int newDebitId = await _banking.CreateRetailDebitOrder(persona.PersonaId, newPremium);
        persona.DebitOrderId = newDebitId;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Received claim for {personaId}, paid out: {claimPaid}", persona.PersonaId, currentlyInsured);
      }
      else
      {
        _logger.LogInformation("Received claim for persona {personaId} that doesn't have insurance", request.PersonaId);
      }

      var myLog = new Log(_simulation.CurrentDate, $"Received claim for persona {request.PersonaId} that doesn't have insurance");
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

      var newPremium = Insurance.CalculateInsurance(currentPersona.Electronics);
      int newDebitId = await _banking.CreateRetailDebitOrder(currentPersona.PersonaId, newPremium);
      currentPersona.DebitOrderId = newDebitId;
      await _context.SaveChangesAsync();

      var myLog = new Log(_simulation.CurrentDate, $"Received new insurance request ({request.AmountNew} electronics) for {currentPersona.PersonaId}");
      loggerContext.Add(myLog);
      await loggerContext.SaveChangesAsync();
      _logger.LogInformation("Received new insurance request ({AmountNew} electronics) for {personaId}", request.AmountNew, currentPersona.PersonaId);
    }

    return NoContent();
  }

  /// <summary>
  /// Endpoint called when price of insurance changes.
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  [HttpPatch("price")]
  public async Task<ActionResult> UpdatePrice([FromBody] ModifyInsurancePrice request)
  {
    Insurance.CurrentPrice = (long)request.NewPrice;
    _logger.LogInformation("Received new price for insurance: {CurrentPrice}", Insurance.CurrentPrice);

    var myLog = new Log(_simulation.CurrentDate, $"Received new price for insurance: {Insurance.CurrentPrice}");
    loggerContext.Add(myLog);
    await loggerContext.SaveChangesAsync();
    return NoContent();
  }
}