namespace Backend.Types.Endpoint
{
  public record Persona(int Id, int personaID, int electronics, bool blacklisted)
  {
    // Parameterless constructor
    public Persona() : this(default, default, default, default)
    {
    }
  }
}
