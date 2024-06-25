namespace Backend.Models;

public class TimeNotification(string newTime)
{
  public string CurrentTime { get; set;} = newTime;
}