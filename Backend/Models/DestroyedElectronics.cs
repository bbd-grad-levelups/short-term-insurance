
namespace Backend.Models;

public class DestroyedElectronics(long personaId, int amountDestroyed)
{
  public long PersonaId { get; set; } = personaId;
  public int AmountDestroyed { get; set; } = amountDestroyed;
}
