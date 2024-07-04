using Backend.Contexts;
using Backend.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

public record PersonaPage(IEnumerable<Persona> Personas, int Page, int PageSize, int AvailablePages);
public record LogsPage(IEnumerable<Log> Logs, int Page, int PageSize, int AvailablePages);

[Route("api")]
[ApiController]
public class FrontendController(PersonaContext personaContext, LoggerContext loggerContext) : ControllerBase
{
  private readonly PersonaContext _personaContext = personaContext;
  private readonly LoggerContext _loggerContext = loggerContext;

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
  public async Task<ActionResult<PersonaPage>> GetPersonas(int page = 1, int pageSize = 50)
  {
    var totalPersonas = await _personaContext.Personas.CountAsync();
    var totalPages = (int)Math.Ceiling(totalPersonas / (double)pageSize);
    var personas = await _personaContext.Personas
                                .OrderBy(x => x.PersonaId)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

    IEnumerable<Persona> results;
    if (personas.Count == 0)
    {
      results = [];
    }
    else
    {
      results = personas;
    }

    var personaPage = new PersonaPage(results, page, pageSize, totalPages);

    return Ok(personaPage);
  }

  /// <summary>
  /// Retrieves a list of logs, paginated.
  /// Admin frontend only, not useful to other services.
  /// Times should be in format YYYY|MM|DD
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
  public async Task<ActionResult<LogsPage>> GetLog(string beginDate, string endDate, int page = 1, int pageSize = 50)
  {
    beginDate = beginDate[2..].Replace('/', '|');
    endDate = endDate[2..].Replace('/', '|');
    var totalCount = await _loggerContext.Logs
      .Where(x => string.Compare(x.TimeStamp, beginDate) >= 0 && string.Compare(x.TimeStamp, endDate) <= 0)
      .CountAsync();
    var logs = await _loggerContext.Logs
                                .Where(x => string.Compare(x.TimeStamp, beginDate) >= 0 && string.Compare(x.TimeStamp, endDate) <= 0)
                                .OrderBy(x => x.TimeStamp)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

    IEnumerable<Log> results;
    if (logs.Count == 0)
    {
      results = [];
    }
    else
    {
      results = logs;
    }
    
    var logsPage = new LogsPage(results, page, pageSize, totalPages);

    return Ok(logsPage);
  }
}

