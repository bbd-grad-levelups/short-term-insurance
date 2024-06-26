namespace Backend.Models;

public class ModifyInsurancePrice(double amount)
{
  public double NewPrice { get; set; } = amount;
}