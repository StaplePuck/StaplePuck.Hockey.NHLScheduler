
namespace StaplePuck.Hockey.NHLScheduler.NHL
{
    public interface INHLClient
    {
        Task<ScoreDateResult?> GetGamesOnDateAsync(string dateId, CancellationToken cancellationToken);
    }
}