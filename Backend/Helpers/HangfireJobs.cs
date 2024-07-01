using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Jobs;

public class HangfireJobs(IStockExchangeService stock, IBankingService banking, ISimulationService sim, ILogger<HangfireJobs> logger) : ControllerBase
{
  private readonly IStockExchangeService _stock = stock;
  private readonly ILogger<HangfireJobs> _logger = logger;
  private readonly ISimulationService _simulation = sim;
  private readonly IBankingService _banking = banking;

  public async Task TimeStep()
  {
    _logger.LogInformation("Updating time.");

    if (_simulation.IsRunning)
    {
      var events = _simulation.UpdateDate();

      if (events.NewMonth)
      {
        float profit = await _banking.RequestProfit();
        await _stock.RequestDividends(profit);
        _logger.LogInformation("Requested Dividends payout for monthly profit: {profit}", profit);
      }

      if (events.NewYear)
      {
        // pay tax
      }
    }
  }
}
