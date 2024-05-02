
namespace StaplePuck.Hockey.NHLScheduler
{
    public interface IScheduler
    {
        Task CreateSchedulesAsync(CancellationToken cancellationToken);
    }
}