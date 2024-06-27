namespace Backend.Helpers;

public record TimeEvents(bool NewMonth, bool NewYear);

public interface ISimulationService 
{
  bool IsRunning { get; set; }
  string CurrentDate { get; set; }

  void StartSim();
  //TimeEvents SetDate(string newDate);
  TimeEvents UpdateDate();
  void Reset();

}

public class SimulationService() : ISimulationService
{
  internal DateTime simStart;
  public string CurrentDate { get; set; } = "01|01|01";

  public bool IsRunning { get; set; } = false;

  //public TimeEvents SetDate(string newDate)
  //{
  //  simStart = CalculateStartTime(newDate);
  //  return new TimeEvents(false, false);
  //}

  public TimeEvents UpdateDate()
  {
    var newDate = CalculateTime();
    TimeEvents events = CompareDates(CurrentDate, newDate);
    CurrentDate = newDate;

    return events;
  }

  internal string CalculateTime()
  {
    var timePassed = DateTime.Now - simStart;

    int simDaysPassed = (int)Math.Round(timePassed.TotalMinutes) / 2;

    int years = simDaysPassed / (12 * 30);
    int months = (simDaysPassed % (12 * 30)) / 30;
    int days = (simDaysPassed % (12 * 30)) % 30;

    // Format the date as YY|MM|DD
    string formattedDate = $"{years:00}|{months + 1:00}|{days + 1:00}";
    return formattedDate;
  }

  //internal DateTime CalculateStartTime(string simDate)
  //{
  //  var parts = simDate.Split('|');
  //  int years = int.Parse(parts[0]) - 1;
  //  int months = int.Parse(parts[1]) - 1; // Subtract 1 to convert to zero-based index
  //  int days = int.Parse(parts[2]) - 1;   // Subtract 1 to convert to zero-based index
  //  int simDaysPassed = years * 12 * 30 + months * 30 + days;

  //  return DateTime.Now - TimeSpan.FromMinutes(simDaysPassed * 2);
  //}

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

    return new TimeEvents(isNewMonth, isNewYear);
  }

  public void Reset()
  {
    IsRunning = false;
  }

  public void StartSim()
  {
    IsRunning = true;
    CurrentDate = "01|01|01";
  }
}