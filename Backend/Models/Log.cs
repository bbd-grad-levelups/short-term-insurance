namespace Backend.Models;

public class Log(string timeStamp, string message) 
{
  public string TimeStamp { get; set; } = timeStamp;
  public string Message { get; set; } = message;
}