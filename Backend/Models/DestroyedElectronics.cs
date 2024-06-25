
namespace Backend.Models;

public class DestroyedElectronics(long persona, int destroyed)
{
  public long PersonaId { get; set; } = persona;
  public int AmountDestroyed { get; set; } = destroyed;
}
