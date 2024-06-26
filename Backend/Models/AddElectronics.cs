namespace Backend.Models;

public class AddElectronics(long personaId, int amountNew)
{
  public long PersonaId { get; set; } = personaId;
  public int AmountNew { get; set; } = amountNew;
}