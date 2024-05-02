
namespace StaplePuck.Hockey.NHLScheduler.Schedule
{
    public interface IScheduleClient
    {
        Task CreateDayAfterScheduleAsync(DateDetails dateDetails, CancellationToken cancellationToken);
        Task CreateGamesScheduleAsync(DateDetails dateDetails, DateTime firstStart, DateTime lastStart, CancellationToken cancellationToken);
        Task CreateStateScheduleAsync(DateDetails dateDetails, CancellationToken cancellationToken);
    }
}