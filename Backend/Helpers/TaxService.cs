namespace Backend.Services;

record RegisterTaxBody(string BusinessName);

public interface ITaxService
{
  Task Register();
  public long Profit { get; set; }
}

public class TaxService(ILogger<TaxService> logger) : BaseService(logger), ITaxService
{
  private string taxId = "";

  public async Task Register()
  {
    var body = new RegisterTaxBody("short_term_insurance");
    var apiUrl = $"{_taxEndpoint}/api/taxpayer/business/register";
    var response = await PerformCall("tax-service", apiUrl, body, HttpMethod.Post);

    taxId = await response.Content.ReadAsStringAsync();
  }

  public long Profit { get; set; }
}