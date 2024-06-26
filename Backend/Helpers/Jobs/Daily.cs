namespace Backend.Helpers.Jobs;

public class HangfireJobs
{
    public void RegisterCompany(int amount)
    {
        // Your job logic here
        Console.WriteLine($"Registering company with amount: {amount}");
    }
}
