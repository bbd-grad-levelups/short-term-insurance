using Newtonsoft.Json;

using System.Net.Http.Headers;

namespace Backend.Helpers;

public class StockExchangeSettings
{
    public string? ExchangeKey { get; set; }
    public string? ExchangeEndpoint { get; set; }
}

record RegisterBody(string Name, string BankAccount);
record StockSaleBody(string Company, int Amount);
record DividendsBody(string Company, float Dividends);

public interface IStockExchangeService
{
  Task Register();
  Task SellStock(string tradingId, int amount);
  Task RequestDividends(float profit);
}

public class StockExchangeService : IStockExchangeService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<StockExchangeService> _logger;
  private readonly string _exchangeEndpoint = "https://mese.projects.bbdgrad.com";
  private readonly string registerCallback = "https://api.insurance.projects.bbdgrad.com/api/stock/registered";
  private readonly string dividendsCallback = "https://api.insurance.projects.bbdgrad.com/api/stock/dividends";

  public StockExchangeService(HttpClient httpClient, ILogger<StockExchangeService> logger)
  {
    _logger = logger;

    _httpClient = httpClient;
    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    _httpClient.DefaultRequestHeaders.Add("origin-service", "ShortTermInsurance");
  }

  public async Task Register()
  {

    var body = new RegisterBody("NoChoice", "ShortTermInsurance");
    var jsonBody = JsonConvert.SerializeObject(body);

    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    string apiUrl = $"{_exchangeEndpoint}/businesses?callbackUrl={registerCallback}";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    
    _logger.LogInformation(apiUrl, "", response.StatusCode);
  }

  public async Task RequestDividends(float profit)
  {

    var body = new DividendsBody("ShortTermInsurance", profit);
    var jsonBody = JsonConvert.SerializeObject(body);

    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    string apiUrl = $"{_exchangeEndpoint}/sell?callbackUrl={dividendsCallback}";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    _logger.LogInformation(apiUrl, "", response.StatusCode);
  }

  public async Task SellStock(string tradingId, int amount) 
  {

    var body = new StockSaleBody("ShortTermInsurance", amount);
    var jsonBody = JsonConvert.SerializeObject(body);

    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    string apiUrl = $"{_exchangeEndpoint}/sell";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    _logger.LogInformation(apiUrl, "", response.StatusCode);
  }
}