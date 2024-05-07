using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaplePuck.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StaplePuck.Hockey.NHLScheduler.NHL;
using StaplePuck.Hockey.NHLScheduler.Schedule;

namespace StaplePuck.Hockey.NHLScheduler;

public class Scheduler : IScheduler
{
    private readonly INHLClient _nhlClient;
    private readonly IScheduleClient _scheduleClient;
    private readonly ILogger _logger;

    public Scheduler(INHLClient nhlClient, IScheduleClient scheduleClient, ILogger<Scheduler> logger)
    {
        _nhlClient = nhlClient;
        _scheduleClient = scheduleClient;
        _logger = logger;
    }

    public static IScheduler Init()
    {
        var builder = new ConfigurationBuilder()
                    .AddEnvironmentVariables();
        var configuration = builder.Build();

        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddOptions()
                    .Configure<Settings>(configuration.GetSection("Settings"))
                    .AddSingleton<INHLClient, NHLClient>()
                    .AddSingleton<IScheduleClient, ScheduleClient>()
                    .AddSingleton<IScheduler, Scheduler>();
            })
            .AddNLog()
            .Build();


        return host.Services.GetRequiredService<IScheduler>();
    }

    public async Task CreateSchedulesAsync(CancellationToken cancellationToken)
    {
        var gameDateId = DateTime.Now.AddDays(1).ToGameDateId();
        var gameDateData = await _nhlClient.GetGamesOnDateAsync(gameDateId, cancellationToken);
        if (gameDateData == null)
        {
            _logger.LogWarning($"Did not get to get date for game date {gameDateId}");
            return;
        }

        var gameGroups = gameDateData.games.Where(x => x.gameType > 1).GroupBy(x => x.gameType);
        foreach (var group in gameGroups)
        {
            var startTimes = group.Select(x => x.startTimeUTC).Where(x => x != DateTimeOffset.MinValue);
            var minStart = startTimes.Min();
            var maxStart = startTimes.Max();
            var game = group.First();
            var isPlayoffs = game.gameType == 3;

            var dateDetails = new DateDetails { SeasonId = game.season.ToString(), GameId = gameDateId, IsPlayoffs = isPlayoffs };
            await _scheduleClient.CreateStateScheduleAsync(dateDetails, cancellationToken);
            await _scheduleClient.CreateGamesScheduleAsync(dateDetails, minStart, maxStart, cancellationToken);
            await _scheduleClient.CreateDayAfterScheduleAsync(dateDetails, cancellationToken);
            _logger.LogInformation($"Created schedules for {gameDateId}. Playoffs: {isPlayoffs}");
        }
    }
}
