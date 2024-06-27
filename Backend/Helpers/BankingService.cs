using Newtonsoft.Json;

using System.Net.Http.Headers;

using Microsoft.Extensions.Options;
using Backend.Models;

namespace Backend.Helpers;

record DebitOrderBody(long PersonaId, string CommercialAccount, double Amount);
record CommercialPaymentBody(string account, string CommercialAccount, double Amount);
record CommercialReferenceBody(string reference);

public interface IBankingService
{
  Task CreateRetailDebitOrder(long personaId, double amount);
  Task MakeCommercialPayment(string account, double amount);
  Task MakeCommercialPayment(long personaId, double amount);
  Task MakeCommercialPayment(string reference);
}

public class BankingService : IBankingService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<BankingService> _logger;
  private readonly string _retailEndpoint;
  internal string _commercialEndpoint;
  internal string _companyAccount;
  internal string _retailKey;
  internal string _commercialKey;

  public BankingService(HttpClient httpClient, IOptions<BankingServiceSettings> options, ILogger<BankingService> logger)
  {
    _retailEndpoint = options.Value.RetailEndpoint ?? throw new Exception("BankingService settings not found");
    _commercialEndpoint = options.Value.CommercialEndpoint ?? throw new Exception("BankingService settings not found");
    _companyAccount = options.Value.CompanyAccount ?? throw new Exception("BankingService settings not found");
    _retailKey = options.Value.RetailKey ?? throw new Exception("BankingService settings not found");
    _commercialKey = options.Value.CommercialKey ?? throw new Exception("BankingService settings not found");
    _companyAccount = options.Value.CompanyAccount ?? throw new Exception("BankingService settings not found");
    _logger = logger;

    _httpClient = httpClient;
    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    _httpClient.DefaultRequestHeaders.Add("origin-service", "ShortTermInsurance");
  }

  public async Task CreateRetailDebitOrder(long personaId, double amount)
  {
    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", _retailKey);

    var body = new DebitOrderBody(personaId, _companyAccount, amount);
    var jsonBody = JsonConvert.SerializeObject(body);

    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    string apiUrl = $"{_retailEndpoint}/debit";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    _logger.LogInformation(response.ToString() + "/n" + response.Content.ToString());
  }

  public async Task MakeCommercialPayment(string account, double amount)
  {
    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", _commercialKey);

    var body = new CommercialPaymentBody(account, _companyAccount, amount);
    var json = JsonConvert.SerializeObject(body);

    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    string apiUrl = _commercialEndpoint + "/pay";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    _logger.LogInformation(response.ToString() + "/n" + response.Content.ToString());
  }

  public async Task MakeCommercialPayment(long personaId, double amount)
  {
    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", _commercialKey);

    var body = new DebitOrderBody(personaId, _companyAccount, amount);
    var json = JsonConvert.SerializeObject(body);

    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    string apiUrl = _commercialEndpoint + "/pay";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    _logger.LogInformation(response.ToString() + "/n" + response.Content.ToString());
  }

  public async Task MakeCommercialPayment(string reference)
  {
    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", _commercialKey);

    var body = new CommercialReferenceBody(reference);
    var json = JsonConvert.SerializeObject(body);

    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    string apiUrl = _commercialEndpoint + "/pay/reference";
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await _httpClient.SendAsync(request);
    _logger.LogInformation(response.ToString() + "/n" + response.Content.ToString());
  }
}