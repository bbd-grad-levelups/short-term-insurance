using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace TodoApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TimeController() : ControllerBase
  {

    [HttpPatch("")]
    public async Task<ActionResult<string>> ReceiveTimeUpdate([FromBody] TimeNotification request)
    {
      if (request.CurrentTime == "newMonth") // However this is done
      {
        // Send out dividends?

        // Send out taxes?
      }

      return Ok("");
    }
  }
}
