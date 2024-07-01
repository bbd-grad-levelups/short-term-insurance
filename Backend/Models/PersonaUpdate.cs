namespace Backend.Models;

public class PersonaUpdate(List<Marriage> marriages, List<ChildRelation> children, List<long> adults, List<Death> deaths)
{
  public List<Marriage> Marriages { get; set; } = marriages;
  public List<ChildRelation> Children { get; set; } = children;
  public List<long> Adults { get; set; } = adults;
  public List<Death> Deaths { get; set; } = deaths;
}

public class Marriage
{
  public long PartnerA { get; set; }
  public long PartnerB { get; set; }
}

public class ChildRelation
{
  public long Parent { get; set; }
  public long Child { get; set; }
}

public class Death
{
  public long Deceased { get; set; }
  public long NextOfKin { get; set; }
}
