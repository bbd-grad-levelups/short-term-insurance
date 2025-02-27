namespace Backend.Services;

public record TimeEvents(bool NewMonth, bool NewYear, string NewDate);

public interface ISimulationService
{
  bool IsRunning { get; set; }
  string CurrentDate { get; }

  void StartSim(DateTime startTime);
  TimeEvents UpdateDate();
  void Reset();
  int DaysSinceDate(string date);
}

public class SimulationService() : ISimulationService
{

  internal DateTime simStart;
  public string CurrentDate { get; set; } = "01|01|01";

  public bool IsRunning { get; set; } = false;

  public TimeEvents UpdateDate()
  {
    var newDate = CalculateTime();
    TimeEvents events = CompareDates(CurrentDate, newDate);
    CurrentDate = newDate;
    return new TimeEvents(events.NewMonth, events.NewYear, newDate);
  }

  internal string CalculateTime()
  {
    var timePassed = DateTime.Now - simStart;

    int simDaysPassed = (int)Math.Round(timePassed.TotalMinutes) / 2;

    int years = simDaysPassed / (12 * 30);
    int months = simDaysPassed % (12 * 30) / 30;
    int days = simDaysPassed % (12 * 30) % 30;

    // Format the date as YY|MM|DD
    string formattedDate = $"{years + 1:00}|{months + 1:00}|{days + 1:00}";
    return formattedDate;
  }

  public TimeEvents CompareDates(string oldDate, string newDate)
  {
    var oldParts = oldDate.Split('|');
    var newParts = newDate.Split('|');

    int oldYear = int.Parse(oldParts[0]);
    int oldMonth = int.Parse(oldParts[1]);
    int newYear = int.Parse(newParts[0]);
    int newMonth = int.Parse(newParts[1]);

    bool isNewMonth = newMonth != oldMonth;
    bool isNewYear = newYear != oldYear;

    return new TimeEvents(isNewMonth, isNewYear, "");
  }

  public void Reset()
  {
    IsRunning = false;
  }

  public void StartSim(DateTime startTime)
  {
    IsRunning = true;
    simStart = startTime;
    TimeEvents events = UpdateDate();
    CurrentDate = events.NewDate;
  }

  public int DaysSinceDate(string date)
  {
    return ConvertToDays(CurrentDate) - ConvertToDays(date);
  }

  internal static int ConvertToDays(string date)
  {
    var dateParts = date.Split('|');
    
    int years = int.Parse(dateParts[0]);
    int months = int.Parse(dateParts[1]);
    int days = int.Parse(dateParts[2]);

    return (years * 12 + months) * 30 + days;
  }
}