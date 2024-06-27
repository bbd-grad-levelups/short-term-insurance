using Backend.Controllers;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Helpers.Jobs;

public class HangfireJobs(IStockExchangeService stock, IBankingService banking, ISimulationService sim, ILogger<HangfireJobs> logger) : ControllerBase
{
  private readonly IStockExchangeService _stock = stock;
  private readonly ILogger<HangfireJobs> _logger = logger;
  private readonly ISimulationService _simulation = sim;
  private readonly IBankingService _banking = banking;

  public void RegisterCompany(int amount)
  {
    _logger.LogInformation($"Registering company with amount: {amount}");
    _stock.RegisterCompany();
  }

  public async Task TimeStep()
  {
    _logger.LogInformation("Updating time.");

    if (_simulation.IsRunning)
    {
      var events = _simulation.UpdateDate();

      if (events.NewYear)
      {
        // Not sure if we're even doing anything year related?
      }
      else if (events.NewMonth)
      {
        float profit = await _banking.RequestProfit();
        await _stock.RequestDividends(profit);
      }
    }
  }
}
