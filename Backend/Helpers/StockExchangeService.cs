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

public class StockExchangeService(ILogger<StockExchangeService> logger) : BaseService, IStockExchangeService
{
  private readonly ILogger<StockExchangeService> _logger = logger;
  private readonly string registerCallback = "https://api.insurance.projects.bbdgrad.com/api/stock/registered";
  private readonly string dividendsCallback = "https://api.insurance.projects.bbdgrad.com/api/stock/dividends";

  public async Task Register()
  {
    var body = new RegisterBody("NoChoice", "ShortTermInsurance");
    string apiUrl = $"{_exchangeEndpoint}/businesses?callbackUrl={registerCallback}";

    var response = await PerformCall(apiUrl, body, HttpMethod.Post);
    _logger.LogInformation(apiUrl, "", response.StatusCode);
  }

  public async Task RequestDividends(float profit)
  {
    var body = new DividendsBody("ShortTermInsurance", profit);
    string apiUrl = $"{_exchangeEndpoint}/sell?callbackUrl={dividendsCallback}";

    var response = await PerformCall(apiUrl, body, HttpMethod.Post);
    _logger.LogInformation(apiUrl, "", response.StatusCode);
  }

  public async Task SellStock(string tradingId, int amount) 
  {

    var body = new StockSaleBody("ShortTermInsurance", amount);
    string apiUrl = $"{_exchangeEndpoint}/sell";

    var response = await PerformCall(apiUrl, body, HttpMethod.Post);
    _logger.LogInformation(apiUrl, "", response.StatusCode);
  }
}