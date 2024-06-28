namespace Backend.Models;

public class RegisterStockResponse(string id, string name, string bankAccount, int initialStock, int currentStock)
{
  public string Id { get; set; } = id;
  public string Name { get; set; } = name;
  public int InitialStock { get; set; } = initialStock;
  public string BankAccount { get; set; } = bankAccount;
  public int CurrentStock { get; set; } = currentStock;
}