using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Controllers
{
  [Route("api")]
  [ApiController]
  public class FrontendController(PersonaContext context, ILogger<FrontendController> logger) : ControllerBase
  {
    private readonly PersonaContext _context = context;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Retrieves a list of personas, paginated.
    /// Admin frontend only, not useful to other services.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a paginated list of personas.
    /// </remarks>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>A list of personas</returns>
    [HttpGet("personas")]
    public async Task<ActionResult<IEnumerable<Persona>>> GetPersonas(int page = 1, int pageSize = 50)
    {
      var personas = await _context.Personas
                                  .OrderBy(x => x.PersonaId)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

      if (personas.Count == 0)
      {
        return NotFound();
      }

      return Ok(personas);
    }

    // Stubbed for now, possible logging endpoint for frontend

    /// <summary>
    /// Retrieves a list of logs, paginated.
    /// Admin frontend only, not useful to other services.
    /// Times should be in format YYYY|MM|DD|HH|MM|SS
    /// </summary>
    /// <remarks>
    /// This endpoint returns a paginated list of logs, based on search window.
    /// </remarks>
    /// <param name="beginDate">Start time for search</param>
    /// <param name="endDate">End time for search</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>A list of personas</returns>
    [HttpGet("log")]
    public async Task<ActionResult<IEnumerable<Log>>> GetLog(string beginDate, string endDate, int page = 1, int pageSize = 50)
    {
      var someLogs = new List<Log>()
      {
        new("2024|06|24|15|32|40", "Something happened"),
        new("2024|06|24|15|33|32", "Something else happened")
      };

      return Ok(someLogs);
    }
  }
}
