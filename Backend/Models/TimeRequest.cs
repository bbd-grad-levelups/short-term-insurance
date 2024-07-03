namespace Backend.Models;

public class TimeRequest
{
  public string Action { get; set; }
  public DateTime? StartTime { get; set; }

  public TimeRequest(string action, DateTime startTime)
  {
    Action = action;
    StartTime = startTime;
  }

  public TimeRequest()
  {
    Action = "start";
    StartTime = null;
  }
}