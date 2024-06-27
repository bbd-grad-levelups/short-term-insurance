using Newtonsoft.Json;

using System.Net.Http.Headers;

using Microsoft.Extensions.Options;

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
  Task RegisterCompany();
  Task SellStock(int amount);
  Task RequestDividends(float profit);
}

public class StockExchangeService : IStockExchangeService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<StockExchangeService> _logger;
  private readonly string _exchangeEndpoint;
  internal string _exchangeKey;

  public StockExchangeService(HttpClient httpClient, IOptions<StockExchangeSettings> options, ILogger<StockExchangeService> logger)
  {
    _exchangeEndpoint = options.Value.ExchangeEndpoint ?? throw new Exception("StockExchange settings not found");
    _exchangeKey = options.Value.ExchangeKey ?? throw new Exception("StockExchange settings not found");
    _logger = logger;

    _httpClient = httpClient;
    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    _httpClient.DefaultRequestHeaders.Add("origin-service", "ShortTermInsurance");
  }

  public async Task RegisterCompany()
  {
    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", _exchangeKey);

    var body = new RegisterBody("NoChoice", "ShortTermInsurance");
    var jsonBody = JsonConvert.SerializeObject(body);

    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    string apiUrl = $"{_exchangeEndpoint}/businesses?callbackUrl=blah";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    
    _logger.LogInformation(apiUrl, "", response.StatusCode);
  }

  public async Task RequestDividends(float profit)
  {
    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", _exchangeKey);

    var body = new DividendsBody("ShortTermInsurance", profit);
    var jsonBody = JsonConvert.SerializeObject(body);

    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    string apiUrl = $"{_exchangeEndpoint}/sell?callbackUrl=blah";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    _logger.LogInformation(apiUrl, "", response.StatusCode);
  }

  public async Task SellStock(int amount) 
  {
    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", _exchangeKey);

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