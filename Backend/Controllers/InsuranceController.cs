using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Helpers;
using Backend.Controllers;

namespace Backend.Controllers
{
  [Route("api/insurance")]
  [ApiController]
  public class InsuranceController(PersonaContext context, IStockExchangeService stock, IBankingService banking, ILogger<InsuranceController> logger) : ControllerBase
  {
    private readonly PersonaContext _context = context;
    private readonly IStockExchangeService _stock = stock;
    private readonly ILogger<InsuranceController> _logger = logger;
    private readonly IBankingService _banking = banking;

    [HttpPost("registered")]
    public async Task<ActionResult> ReceiveStockRegistration([FromBody] RegisterStockResponse request)
    {
      await _stock.SellStock(10000);
      return Ok();
    }

    [HttpPost("dividends")]
    public async Task<ActionResult> ReceiveDividendsReference([FromBody] DividendsResponse request)
    {
      _banking.MakeCommercialPayment(request.Reference);
      return Ok();
    }
  }
}