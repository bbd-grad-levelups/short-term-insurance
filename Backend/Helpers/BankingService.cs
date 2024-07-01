using Newtonsoft.Json;

using System.Net.Http.Headers;

namespace Backend.Helpers;

record DebitOrderBody(long PersonaId, string CommercialAccount, double Amount);
record CommercialPaymentBody(string Account, string CommercialAccount, double Amount);
record CommercialReferenceBody(string Reference);

public interface IBankingService
{
  Task<int> CreateRetailDebitOrder(long personaId, double amount);
  Task MakeCommercialPayment(string account, double amount);
  Task MakeCommercialPayment(long personaId, double amount);
  Task MakeCommercialPayment(string reference);
  Task<float> RequestProfit();
}

public class BankingService : IBankingService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<BankingService> _logger;
  private readonly string _retailEndpoint = "https://api.retailbank.projects.bbdgrad.com";
  private readonly string _commercialEndpoint = "https://api.commercialbank.projects.bbdgrad.com";
  private readonly string _companyAccount = "short-term-insurance";

  public BankingService(HttpClient httpClient, ILogger<BankingService> logger)
  {
    _logger = logger;

    _httpClient = httpClient;
    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    _httpClient.DefaultRequestHeaders.Add("origin-service", "ShortTermInsurance");
  }

  public async Task<int> CreateRetailDebitOrder(long personaId, double amount)
  {

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
    return 1; // TODO: ACTUALLY DO LOL
  }

  public async Task MakeCommercialPayment(string account, double amount)
  {

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

  public async Task<float> RequestProfit()
  {

    var body = new CommercialReferenceBody("");
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

    return float.Parse(response.Content.ToString());
  }
}