using Backend.Contexts;
using Backend.Models;
using Backend.Services;

namespace Backend.Jobs;

public class TimedJobs(LoggerContext loggerCon, IStockExchangeService stock, IBankingService banking, ISimulationService sim, ITaxService tax)
{
  private readonly LoggerContext _loggerContext = loggerCon;
  private readonly IStockExchangeService _stock = stock;
  private readonly ITaxService _taxService = tax;
  private readonly ISimulationService _simulation = sim;
  private readonly IBankingService _banking = banking;

  public async Task TimeStep()
  {
    if (_simulation.IsRunning)
    {
      var logs = new List<Log>();

      var events = _simulation.UpdateDate();
      if (events.NewMonth)
      {

        long dividends = (long)(_taxService.Profit * 0.79);
        long tax = (long)(_taxService.Profit * 0.21);

        await _stock.RequestDividends(dividends);
        await _banking.MakeCommercialPayment("central-revenue-service", "short term insurance tax", tax);
        _taxService.Profit = 0;

        logs.Add(new Log(events.NewDate, $"Requested Dividends payout for monthly profit: {dividends}"));
        logs.Add(new Log(events.NewDate, $"Paid tax for month {events.NewDate}: {tax}"));
      }
      else if (events.NewYear)
      {
        logs.Add(new Log(events.NewDate, "Happy new year!"));
      }

      _loggerContext.Logs.AddRange(logs);
      await _loggerContext.SaveChangesAsync();
    }
  }
}
