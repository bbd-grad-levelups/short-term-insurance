namespace Backend.Models;

public class ModifyElectronicsPrice(double amount)
{
  public double ElectronicsPrice { get; set; } = amount;
}