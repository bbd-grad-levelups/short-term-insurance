namespace Backend.Services;

record RegisterTaxBody(string BusinessName);
record RequestTaxBody(string TaxId);

public interface ITaxService
{
  Task Register();
  Task PayTax();
  public long Profit { get; set; }
}

public class TaxService(ILogger<TaxService> logger) : BaseService(logger), ITaxService
{
  private string taxId = "";

  public async Task Register()
  {
    var body = new RegisterTaxBody("short-term-insurance");
    var apiUrl = $"{_taxEndpoint}/api/taxpayer/business/register";
    var response = await PerformCall("tax-service", apiUrl, body, HttpMethod.Post);

    taxId = await response.Content.ReadAsStringAsync();
  }

  public async Task PayTax()
  {
    var body = new RequestTaxBody(taxId);
    var apiUrl = $"{_taxEndpoint}/api/taxpayer/getTaxStatement/{taxId}";
    var response = await PerformCall("tax-service", apiUrl, body, HttpMethod.Post);

    // and then pay their bank account?
  }

  public long Profit { get; set; }
}