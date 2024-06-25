
namespace Backend.Models;

public class RegisterElectronics(long persona, string bankAccount, int amount)
{
  public long PersonaId { get; set; } = persona;
  public string BankAccount { get; set; } = bankAccount;
  public int ElectronicsAmount { get; set; } = amount;
}
