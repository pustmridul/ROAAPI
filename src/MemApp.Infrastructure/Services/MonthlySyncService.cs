using Microsoft.Extensions.Hosting;

namespace MemApp.Infrastructure.Services;

public class MonthlySyncService : IHostedService, IDisposable
{
    private Timer _timer;
   // private readonly ISubscription _subscription;
    private readonly IROASubscription _subscription;
    public MonthlySyncService(IROASubscription subscription)
    {
        _subscription = subscription;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(CheckDate, null, TimeSpan.Zero, TimeSpan.FromHours(24));

       // _timer = new Timer(CheckDate, null, TimeSpan.Zero, TimeSpan.FromHours(24));
        return Task.CompletedTask;
    }

    private void CheckDate(object state)
    {
        var currentDate = DateTime.Now;
        if (currentDate.Day == 1)
        {
            DoMonthlySync(currentDate);
        }
    }

    private async void DoMonthlySync(DateTime currentDate)
    {
        await _subscription.MonthlyDueGenerator(currentDate);
        Console.WriteLine("Performing monthly sync at " + currentDate);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}