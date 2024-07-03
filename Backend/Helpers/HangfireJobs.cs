using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Policy;

using Backend.Services;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Backend.Contexts;
using Backend.Controllers;
using Backend.Models;

namespace Backend.Jobs;

public class HangfireJobs(LoggerContext loggerCon, IStockExchangeService stock, IBankingService banking, ISimulationService sim, ITaxService tax, ILogger<HangfireJobs> logger) : ControllerBase
{
  private readonly LoggerContext _loggerContext = loggerCon;
  private readonly IStockExchangeService _stock = stock;
  private readonly ILogger<HangfireJobs> _logger = logger;
  private readonly ITaxService _taxService = tax;
  private readonly ISimulationService _simulation = sim;
  private readonly IBankingService _banking = banking;

  public async Task TimeStep()
  {
    _logger.LogInformation("Updating time.");

    if (_simulation.IsRunning)
    {
      var logs = new List<Log>();

      var events = _simulation.UpdateDate();
      _logger.LogInformation("Current time: {newDate}", events.NewDate);
      if (events.NewMonth)
      {
        _logger.LogInformation("New month!");

        long dividends = (long)(_taxService.Profit * 0.79);
        long tax = (long)(_taxService.Profit * 0.21);

        await _stock.RequestDividends(dividends);
        await _banking.MakeCommercialPayment("central-revenue-service", tax);
        _taxService.Profit = 0;
        _logger.LogInformation("Requested Dividends payout for monthly profit: {dividends}", dividends);
        _logger.LogInformation("Paid tax for month {NewDate}: {tax}", events.NewDate, tax);
        logs.Add(new Log(events.NewDate, $"Requested Dividends payout for monthly profit: {dividends}"));
        logs.Add(new Log(events.NewDate, $"Paid tax for month {events.NewDate}: {tax}"));
      }
      else if (events.NewYear)
      {
        _logger.LogInformation("Happy new year!");
        logs.Add(new Log(events.NewDate, "Happy new year!"));
      }

      _loggerContext.Logs.AddRange(logs);
      await _loggerContext.SaveChangesAsync();
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
