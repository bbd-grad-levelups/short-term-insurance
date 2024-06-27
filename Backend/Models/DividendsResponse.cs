
namespace Backend.Models;

public class DividendsResponse(string referenceId)
{
  public string ReferenceId { get; set; } = referenceId;
}