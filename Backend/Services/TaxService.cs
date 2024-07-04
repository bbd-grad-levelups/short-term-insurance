using Newtonsoft.Json;

namespace Backend.Services;

record RegisterTaxBody(string BusinessName);
record RegisterTaxResponse(string TaxId);

public interface ITaxService
{
  Task Register();
  public long Profit { get; set; }
  public string TaxId { get; set; }
}

public class TaxService(ILogger<TaxService> logger) : BaseService(logger), ITaxService
{
  public string TaxId { get; set; } = "short_term_insurance";
  public long Profit { get; set; }

  public async Task Register()
  {
    var body = new RegisterTaxBody("short_term_insurance");
    var apiUrl = $"{_taxEndpoint}/api/taxpayer/business/register";
    var response = await PerformCall("tax-service", apiUrl, body, HttpMethod.Post);

    if (response.IsSuccessStatusCode)
    {
      var responseBody = await response.Content.ReadAsStringAsync();
      var taxResponse = JsonConvert.DeserializeObject<RegisterTaxResponse>(responseBody);

      TaxId = taxResponse?.TaxId ?? "short_term_insurance";
    }
    else
    {
      TaxId = "short_term_insurance";
    }
  } 
}