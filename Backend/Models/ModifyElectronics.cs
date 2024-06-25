namespace Backend.Models;

public class ModifyElectronics(long persona, int destroyed, int gained)
{
  public long PersonaId { get; set; } = persona;
  public int AmountDestroyed { get; set; } = destroyed;
  public int AmountNew { get; set; } = gained;
}
