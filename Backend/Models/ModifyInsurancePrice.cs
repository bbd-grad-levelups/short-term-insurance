namespace Backend.Models;

public class ModifyInsurancePrice(double newPrice)
{
  public double NewPrice { get; set; } = newPrice;
}