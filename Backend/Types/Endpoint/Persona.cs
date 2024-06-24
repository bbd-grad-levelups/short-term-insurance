namespace Backend.Types.Endpoint
{
  public record Persona(int Id, int PersonaID, int Electronics, bool Blacklisted)
  {
    // Parameterless constructor
    public Persona() : this(default, default, default, default)
    {
    }
  }
}
