namespace Backend.Services;

public class DebitOrderBody(long amountInMibiBBDough, long personaId)
{
  public long AmountInMibiBBDough { get; set; } = amountInMibiBBDough;
  public long PersonaId { get; set; } = personaId;
  public int DayInMonth = 1;
  public string EndsAt = "99|99|99";
  public Recepient Recepient  = new();
}

public class Recepient
{
  public int BankId = 1001;
  public string AccountId = "short-term-insurance";
}

record CancelDebitOrderBody(int DebitOrderId);
record CommercialPaymentBody(string Account, string CommercialAccount, double Amount);
record CommercialReferenceBody(string Reference);

public interface IBankingService
{
  Task CancelDebitOrder(int debitOrderId);
  Task<int> CreateRetailDebitOrder(long personaId, long amount);
  Task MakeCommercialPayment(string account, long amount);
  Task MakeCommercialPayment(long personaId, long amount);
  Task MakeCommercialPayment(string reference);
  Task<float> RequestProfit();
}

public class BankingService(ILogger<BankingService> logger) : BaseService(logger), IBankingService
{
  private readonly string _companyAccount = "short-term-insurance";

  public async Task CancelDebitOrder(int debitOrderId)
  {
    var body = new object();
    string apiUrl = $"{_retailEndpoint}/api/debitorders?{debitOrderId}";
    await PerformCall("banking-service", apiUrl, body, HttpMethod.Delete);
  }

  public async Task<int> CreateRetailDebitOrder(long personaId, long amount)
  {
    var body = new DebitOrderBody(amount, personaId);
    string apiUrl = $"{_retailEndpoint}/api/debitorders";
    await PerformCall("banking-service", apiUrl, body, HttpMethod.Post);
    return 1; // TODO: ACTUALLY DO LOL
  }

  public async Task MakeCommercialPayment(string account, long amount)
  {
    var body = new CommercialPaymentBody(account, _companyAccount, amount);
    string apiUrl = $"{_commercialEndpoint}/pay";
    await PerformCall("banking-service", apiUrl, body, HttpMethod.Post);
  }

  public async Task MakeCommercialPayment(long personaId, long amount)
  {
    var body = new CommercialPaymentBody(personaId.ToString(), _companyAccount, amount);
    string apiUrl = $"{_commercialEndpoint}/pay";
    await PerformCall("banking-service", apiUrl, body, HttpMethod.Post);
  }

  public async Task MakeCommercialPayment(string reference)
  {
    var body = new CommercialReferenceBody(reference);
    string apiUrl = $"{_commercialEndpoint}/pay/reference";
    await PerformCall("banking-service", apiUrl, body, HttpMethod.Post);
  }

  public async Task<float> RequestProfit()
  {
    var body = new CommercialReferenceBody("");
    string apiUrl = $"{_commercialEndpoint}/pay/reference";
    var response = await PerformCall("banking-service", apiUrl, body, HttpMethod.Post);

    return float.Parse(response.Content?.ToString());
  }
}