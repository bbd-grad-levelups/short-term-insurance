
namespace Backend.Models;

public class BankNotification(string type, TransactionInfo transactionInfo)
{
  public string Type { get; set; } = type;
  public TransactionInfo Transaction { get; set; } = transactionInfo;
}

public class TransactionInfo(string id, string debitAccountName, string creditAccountName, 
  long amount, string status, string reference, string date)
{
  public string Id { get; set; } = id;
  public string DebitAccountName { get; set; } = debitAccountName;
  public string CreditAccountName { get; set; } = creditAccountName;
  public decimal Amount { get; set; } = amount;
  public string Status { get; set; } = status;
  public string Reference { get; set; } = reference;
  public string Date { get; set; } = date;
}

