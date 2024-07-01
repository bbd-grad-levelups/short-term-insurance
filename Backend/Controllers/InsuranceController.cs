using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Helpers;

namespace Backend.Controllers;

[Route("api/insurance")]
[ApiController]
public class InsuranceController(IStockExchangeService stock, IBankingService banking, ILogger<InsuranceController> logger) : ControllerBase
{
  private readonly IStockExchangeService _stock = stock;
  private readonly ILogger<InsuranceController> _logger = logger;
  private readonly IBankingService _banking = banking;

  /// <summary>
  /// Callback endpoint for registering business on the stock exchange.
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  [HttpPost("registered")]
  public async Task<ActionResult> ReceiveStockRegistration([FromBody] RegisterStockResponse request)
  {
    _logger.LogInformation("Stock Exchange registration successful (ID: {tradingId}). Registering initial company stock on the Stock Exchange", request.TradingId);
    
    await _stock.SellStock(request.TradingId, 10000);
    return Ok();
  }

  /// <summary>
  /// Callback endpoint for receiving payment reference to sell stock.
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  [HttpPost("dividends")]
  public async Task<ActionResult> ReceiveDividendsReference([FromBody] DividendsResponse request)
  {
    _logger.LogInformation("Received dividends payment reference {ReferenceId}, making payment", request.ReferenceId);

    await _banking.MakeCommercialPayment(request.ReferenceId);
    return Ok();
  }
}