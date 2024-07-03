using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Policy;

using Backend.Services;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Backend.Jobs;

public class HangfireJobs(IStockExchangeService stock, IBankingService banking, ISimulationService sim, ITaxService tax, ILogger<HangfireJobs> logger) : ControllerBase
{
  private readonly IStockExchangeService _stock = stock;
  private readonly ILogger<HangfireJobs> _logger = logger;
  private readonly ITaxService _taxService = tax;
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
        int profit = _taxService.Profit;
        await _stock.RequestDividends(profit);
        _taxService.Profit = 0;
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

  public async Task CallHealth()
  {
    var client = new HttpClient();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("origin-service", "ShortTermInsurance");


    var response = await client.GetAsync("https://fe.insurance.projects.bbdgrad.com");
  }
}
