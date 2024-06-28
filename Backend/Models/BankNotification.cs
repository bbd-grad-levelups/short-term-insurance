
namespace Backend.Models;

// Not final, pending bank API spec
public class BankNotification(string message, long personaId, bool success)
{
  public string Message { get; set; } = message;
  public long PersonaId { get; set; } = personaId;
  public bool Success { get; set; } = success;
}

