namespace Backend.Helpers;

public static class Insurance
{
  public static double? CurrentPrice { get; set; }

  public static double CalculateInsurance(int electronicsAmount)
  {
    double price;
    if (!CurrentPrice.HasValue)
    {
      // Retrieve from DB, otherwise retrieve from db.
      price = 1;
    }
    else
    {
      price = CurrentPrice.Value;
    }
   
    return electronicsAmount * price * 0.2;
  }
}