namespace Backend.Helpers;

public static class Insurance
{
  public static double CurrentPrice { get; set; }

  public static double CalculateInsurance(int electronicsAmount)
  {
    return electronicsAmount * CurrentPrice * 1.2;
  }

  public static double CalculatePayout(int destroyedAmount)
  {
    return destroyedAmount * CurrentPrice;
  }
}