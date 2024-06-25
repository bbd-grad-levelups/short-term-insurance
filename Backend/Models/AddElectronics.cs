namespace Backend.Models;

public class AddElectronics(long persona, int gained)
{
  public long PersonaId { get; set; } = persona;
  public int AmountNew { get; set; } = gained;
}
