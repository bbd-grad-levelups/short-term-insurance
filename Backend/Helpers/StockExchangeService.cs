using Backend.Models;

namespace Backend.Services;

record RegisterBody(string Name, string BankAccount);
record StockSaleBody(string CompanyId, int Quantity);
record DividendsBody(string BusinessId, long Amount);

public interface IStockExchangeService
{
  Task Register();
  void ReceiveRegistration(RegisterStockResponse response);
  Task SellStock(int amount);
  Task RequestDividends(long profit);
  long GetLastDividends();
}

public class StockExchangeService(ILogger<StockExchangeService> logger) : BaseService(logger), IStockExchangeService
{
  private readonly string registerCallback = "'https://api.insurance.projects.bbdgrad.com/api/stock/registered'";
  private readonly string dividendsCallback = "'https://api.insurance.projects.bbdgrad.com/api/stock/dividends'";
  string tradingId = "";
  string id = "";
  private long lastDividends = 0;

  public long GetLastDividends()
  {
    return lastDividends;
  }

  public void ReceiveRegistration(RegisterStockResponse response)
  {
    tradingId = response.TradingId;
    id = response.Id;
  }

  public async Task Register()
  {
    var body = new RegisterBody("NoChoice", "short_term_insurance");
    string apiUrl = $"{_exchangeEndpoint}/businesses?callbackUrl={registerCallback}";
    await PerformCall("stock-exchange", apiUrl, body, HttpMethod.Post);
  }

  public async Task RequestDividends(long profit)
  {
    lastDividends = profit;
    var body = new DividendsBody(id, profit);
    string apiUrl = $"{_exchangeEndpoint}/dividends?callbackUrl={dividendsCallback}";
    await PerformCall("stock-exchange", apiUrl, body, HttpMethod.Post);
  }

  public async Task SellStock(int amount) 
  {
    var body = new StockSaleBody(tradingId, amount);
    string apiUrl = $"{_exchangeEndpoint}/stocks/sell?callbackUrl=";
    await PerformCall("stock-exchange", apiUrl, body, HttpMethod.Post);
  }

}