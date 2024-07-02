namespace Backend.Services;

record RegisterBody(string Name, string BankAccount);
record StockSaleBody(string Company, int Amount);
record DividendsBody(string Company, float Dividends);

public interface IStockExchangeService
{
  Task Register();
  Task SellStock(string tradingId, int amount);
  Task RequestDividends(float profit);
}

public class StockExchangeService(ILogger<StockExchangeService> logger) : BaseService(logger), IStockExchangeService
{
  private readonly string registerCallback = "https://api.insurance.projects.bbdgrad.com/api/stock/registered";
  private readonly string dividendsCallback = "https://api.insurance.projects.bbdgrad.com/api/stock/dividends";

  public async Task Register()
  {
    var body = new RegisterBody("NoChoice", "ShortTermInsurance");
    string apiUrl = $"{_exchangeEndpoint}/businesses?callbackUrl={registerCallback}";
    await PerformCall("stock-exchange", apiUrl, body, HttpMethod.Post);
  }

  public async Task RequestDividends(float profit)
  {
    var body = new DividendsBody("ShortTermInsurance", profit);
    string apiUrl = $"{_exchangeEndpoint}/dividends?callbackUrl={dividendsCallback}";
    await PerformCall("stock-exchange", apiUrl, body, HttpMethod.Post);
  }

  public async Task SellStock(string tradingId, int amount) 
  {
    var body = new StockSaleBody("ShortTermInsurance", amount);
    string apiUrl = $"{_exchangeEndpoint}/stocks/sell?callBackUrl=";
    await PerformCall("stock-exchange", apiUrl, body, HttpMethod.Post);
  }

}