

namespace Backend.Models;

public class TaxNotification(double amount, string bankAccount)
{
  public double Amount { get; set; } = amount;
  
  public string BankAccount { get; set; } = bankAccount;
}