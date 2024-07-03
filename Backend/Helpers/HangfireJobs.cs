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
    Console.WriteLine("Updating time: hangfire job");

    if (_simulation.IsRunning)
    {
      var events = _simulation.UpdateDate();
      _logger.LogInformation("Current time: {newDate}", events.NewDate);
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

  public async Task<string> TestEndpoints()
  {
    //await _stock.Register();
    //await _stock.SellStock("rr", 100);
    //await _stock.RequestDividends(1000);
    //await _banking.CreateRetailDebitOrder(1, 11);
    return "Registration completed";
  }
}
