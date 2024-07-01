namespace Backend.Models;

public class RegisterStockResponse(string id, string name, string bankAccount, string tradingId)
{
  public string Id { get; set; } = id;
  public string Name { get; set; } = name;
  public string BankAccount { get; set; } = bankAccount;
  public string TradingId { get; set; } = tradingId;
}