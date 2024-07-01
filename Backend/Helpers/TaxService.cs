using Newtonsoft.Json;

using System.Net.Http.Headers;

namespace Backend.Helpers;

record RegisterTaxBody(string BusinessName);
record RequestTaxBody(string TaxId);

public interface ITaxService
{
  Task Register();
  Task PayTax();
}

public class TaxService : ITaxService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<StockExchangeService> _logger;
  private readonly string _exchangeEndpoint = "https://mese.projects.bbdgrad.com";

  private string taxId;

  public TaxService(HttpClient httpClient, ILogger<StockExchangeService> logger)
  {
    _logger = logger;

    _httpClient = httpClient;
    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    _httpClient.DefaultRequestHeaders.Add("origin-service", "ShortTermInsurance");
    taxId = "";
    // Add cert here

  }

  public async Task Register()
  {
    var body = new RegisterTaxBody("short-term-insurance");
    var jsonBody = JsonConvert.SerializeObject(body);

    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    var apiUrl = $"{_exchangeEndpoint}/api/taxpayer/business/register";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    taxId = await response.Content.ReadAsStringAsync();
  }

  public async Task PayTax()
  {
    var body = new RequestTaxBody(taxId);
    var jsonBody = JsonConvert.SerializeObject(body);

    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    var apiUrl = $"{_exchangeEndpoint}/api/taxpayer/getTaxStatement/{taxId}";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    // and then pay their bank account?
  }
}