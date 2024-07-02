namespace Backend.Helpers;

public static class Insurance
{
  public static long CurrentPrice { get; set; }

  public static long CalculateInsurance(int electronicsAmount)
  {
    return (long)Math.Round(electronicsAmount * CurrentPrice * 1.2);
  }

  public static long CalculatePayout(int destroyedAmount)
  {
    return destroyedAmount * CurrentPrice;
  }
}