using Backend.Models;

using Microsoft.EntityFrameworkCore;

namespace Backend.Contexts;

public class LoggerContext(DbContextOptions<LoggerContext> options) : DbContext(options)
{
  public DbSet<Log> Logs { get; set; } = null!;

  public async Task RemoveAll()
  {
    await Database.ExecuteSqlRawAsync("DELETE FROM logs");
  }
}