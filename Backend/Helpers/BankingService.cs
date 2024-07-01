namespace Backend.Services;

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

public class BankingService(ILogger<BankingService> logger) : BaseService, IBankingService
{
  private readonly ILogger<BankingService> _logger = logger;
  private readonly string _companyAccount = "short-term-insurance";

  public async Task<int> CreateRetailDebitOrder(long personaId, double amount)
  {
    // TODO: Cancel existing debit order first! haha

    var body = new DebitOrderBody(personaId, _companyAccount, amount);
    string apiUrl = $"{_retailEndpoint}/debit";
    var response = await PerformCall(apiUrl, body, HttpMethod.Post);

    _logger.LogInformation(response.StatusCode + "/n" + response.Content.ToString());
    return 1; // TODO: ACTUALLY DO LOL
  }

  public async Task MakeCommercialPayment(string account, double amount)
  {
    var body = new CommercialPaymentBody(account, _companyAccount, amount);
    string apiUrl = $"{_commercialEndpoint}/pay";

    var response = await PerformCall(apiUrl, body, HttpMethod.Post);
    _logger.LogInformation(response.ToString() + "/n" + response.Content.ToString());
  }

  public async Task MakeCommercialPayment(long personaId, double amount)
  {
    var body = new DebitOrderBody(personaId, _companyAccount, amount);
    string apiUrl = $"{_commercialEndpoint}/pay";

    var response = await PerformCall(apiUrl, body, HttpMethod.Post);
    _logger.LogInformation(response.ToString() + "/n" + response.Content.ToString());
  }

  public async Task MakeCommercialPayment(string reference)
  {
    var body = new CommercialReferenceBody(reference);
    string apiUrl = $"{_commercialEndpoint}/pay/reference";

    var response = await PerformCall(apiUrl, body, HttpMethod.Post);
    _logger.LogInformation(response.ToString() + "/n" + response.Content.ToString());
  }

  public async Task<float> RequestProfit()
  {
    var body = new CommercialReferenceBody("");
    string apiUrl = $"{_commercialEndpoint}/pay/reference";

    var response = await PerformCall(apiUrl, body, HttpMethod.Post);
    _logger.LogInformation(response.ToString() + "/n" + response.Content.ToString());

    return float.Parse(response.Content.ToString());
  }
}