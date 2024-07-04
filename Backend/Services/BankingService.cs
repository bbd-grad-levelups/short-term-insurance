using Newtonsoft.Json;

namespace Backend.Services;

public class DebitOrderBody(long amountInMibiBBDough, long personaId)
{
  public long AmountInMibiBBDough { get; set; } = amountInMibiBBDough;
  public long PersonaId { get; set; } = personaId;
  public int DayInMonth = 1;
  public string EndsAt = "9999-01-01";
  public PaymentRecepient Recepient = new();
}


public class PaymentRecepient
{
  public int BankId = 1001;
  public string AccountId = "short_term_insurance";
}
record DebitOrderResponse(int DebitOrderId);

public class Transaction(string debitAccountName, string creditAccountName, decimal amount, string debitRef, string creditRef)
{
  public string DebitAccountName { get; } = debitAccountName;
  public string CreditAccountName { get; } = creditAccountName;
  public decimal Amount { get; } = amount;
  public string DebitRef { get; } = debitRef;
  public string CreditRef { get; } = creditRef;
}

public class TransactionList(List<Transaction> transactions)
{
  List<Transaction> Transactions { get; set; } = transactions;
}

public interface IBankingService
{
  Task CancelDebitOrder(int debitOrderId);
  Task<int> CreateRetailDebitOrder(long personaId, long amount);
  Task MakeCommercialPayment(string account, string reference, long amount);
}

public class BankingService(ILogger<BankingService> logger) : BaseService(logger), IBankingService
{
  private readonly string _companyAccount = "short_term_insurance";

  public async Task CancelDebitOrder(int debitOrderId)
  {
    var body = new object();
    string apiUrl = $"{_retailEndpoint}/api/debitorders/{debitOrderId}";
    await PerformCall("banking-service", apiUrl, body, HttpMethod.Delete);
  }

  public async Task<int> CreateRetailDebitOrder(long personaId, long amount)
  {
    var body = new DebitOrderBody(amount, personaId);
    string apiUrl = $"{_retailEndpoint}/api/debitorders";
    var response = await PerformCall("banking-service", apiUrl, body, HttpMethod.Post);

    if (response.IsSuccessStatusCode)
    {
      var responseBody = await response.Content.ReadAsStringAsync();
      var debitOrderResponse = JsonConvert.DeserializeObject<DebitOrderResponse>(responseBody);

      if (debitOrderResponse != null)
      {
        return debitOrderResponse.DebitOrderId;
      }
    }

    return 1;
  }

  public async Task MakeCommercialPayment(string account, string reference, long amount)
  {
    var body = new TransactionList([new(_companyAccount, account, amount, $"Paying {account}", reference)]);
    string apiUrl = $"{_commercialEndpoint}/transactions/create";
    await PerformCall("banking-service", apiUrl, body, HttpMethod.Post);
  }
}