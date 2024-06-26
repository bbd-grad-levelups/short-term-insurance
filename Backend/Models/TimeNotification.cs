namespace Backend.Models;

public class TimeNotification(string currentTime)
{
  public string CurrentTime { get; set;} = currentTime;
}