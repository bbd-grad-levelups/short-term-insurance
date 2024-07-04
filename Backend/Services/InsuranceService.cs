using Newtonsoft.Json;

namespace Backend.Services;

public interface IPriceService
{
  public long CurrentPrice {  get; set; }
  public Task<long> UpdatePrice();
  public long CalculateInsurance(int amount);
  public long CalculatePayout(int amount);
}

record PriceResponse(long Value);

public class PriceService(ILogger<PriceService> logger) : BaseService(logger), IPriceService
{
  public long CurrentPrice { get; set; }

  public long CalculateInsurance(int amount)
  {
    return (long)Math.Round(amount * CurrentPrice * 1.2);
  }

  public long CalculatePayout(int amount)
  {
    return amount * CurrentPrice;
  }

  public async Task<long> UpdatePrice()
  {
    long updatedPrice;
    var body = new RegisterBody("NoChoice", "short_term_insurance");
    string apiUrl = $"{_zeusEndpoint}/short-term-insurance";
    var response = await PerformCall("hand-of-zeus", apiUrl, body, HttpMethod.Get);

    if (response.IsSuccessStatusCode)
    {
      var responseBody = await response.Content.ReadAsStringAsync();
      var priceResponse = JsonConvert.DeserializeObject<PriceResponse>(responseBody);

      updatedPrice = priceResponse?.Value ?? 1000;
    }
    else
    {
      updatedPrice = 1000;
    }

    CurrentPrice = updatedPrice;
    return updatedPrice;
  }
}