using Newtonsoft.Json;

using System.Net.Http.Headers;

using Microsoft.Extensions.Options;

namespace Backend.Helpers;

record DebitOrderBody(long PersonaId, string CommercialAccount, double Amount);
record CommercialPaymentBody(string account, string CommercialAccount, double Amount);

public interface IBankingService
{
  Task CreateRetailDebitOrder(long personaId, double amount);
  Task MakeCommercialPayment(string account, double amount);
  Task MakeCommercialPayment(long personaId, double amount);
}

public class BankingService : IBankingService
{
  private readonly HttpClient _httpClient;
  private readonly string _retailEndpoint;
  internal string _commercialEndpoint;
  internal string _companyAccount;
  internal string _retailKey;
  internal string _commercialKey;

  public BankingService(HttpClient httpClient, IOptions<BankingServiceSettings> options)
  {
    _retailEndpoint = options.Value.RetailEndpoint;
    _commercialEndpoint = options.Value.CommercialEndpoint;
    _companyAccount = options.Value.CompanyAccount;
    _retailKey = options.Value.RetailKey;
    _commercialKey = options.Value.CommercialKey;
    _companyAccount = options.Value.CompanyAccount;

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
    Console.WriteLine(response.ToString() + "/n" + response.Content.ToString());
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
    Console.WriteLine(response.ToString() + "/n" + response.Content.ToString());
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
    Console.WriteLine(response.ToString() + "/n" + response.Content.ToString());
  }
}