using Newtonsoft.Json;

using System.Net.Http.Headers;

using Microsoft.Extensions.Options;

namespace Backend.Helpers;

public class StockExchangeSettings
{
    public string ExchangeKey { get; set; }
    public string ExchangeEndpoint { get; set; }
}

// record DebitOrderBody(long PersonaId, string CommercialAccount, double Amount);
record RegisterBody(string Name, string BankAccount);
record StockSaleBody(string Company, int Amount);

record RegisterResponse(string Id, string Name, int BankAccount, int InitialStock);

public interface IStockExchangeService
{
  Task<string?> RegisterCompany();
  Task SellStock(int amount);
}

public class StockExchangeService : IStockExchangeService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<StockExchangeService> _logger;
  private readonly string _exchangeEndpoint;
  internal string _exchangeKey;

  public StockExchangeService(HttpClient httpClient, IOptions<StockExchangeSettings> options, ILogger<StockExchangeService> logger)
  {
    _exchangeEndpoint = options.Value.ExchangeEndpoint;
    _exchangeKey = options.Value.ExchangeKey;
    _logger = logger;

    _httpClient = httpClient;
    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    _httpClient.DefaultRequestHeaders.Add("origin-service", "ShortTermInsurance");
  }

  public async Task<string?> RegisterCompany()
  {
    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", _exchangeKey);

    var body = new RegisterBody("NoChoice", "ShortTermInsurance");
    var jsonBody = JsonConvert.SerializeObject(body);

    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    string apiUrl = $"{_exchangeEndpoint}/businesses";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    string? companyId;
    if (response.IsSuccessStatusCode)
    {
      // Deserialize the JSON response body
      var jsonResponse = await response.Content.ReadAsStringAsync();
      var responseObject = JsonConvert.DeserializeObject<RegisterResponse>(jsonResponse);

      companyId = responseObject?.Id;
    }
    else
    {
      companyId = null;
    }
    
    _logger.LogInformation(apiUrl, "", response.StatusCode);
    return companyId; 
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