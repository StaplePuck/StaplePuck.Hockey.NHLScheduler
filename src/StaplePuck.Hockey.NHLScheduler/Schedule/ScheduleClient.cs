using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Scheduler;
using Amazon.Scheduler.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace StaplePuck.Hockey.NHLScheduler.Schedule
{
    public class ScheduleClient : IScheduleClient
    {
        private readonly Settings _settings;
        private readonly ILogger _logger;
        private readonly IAmazonScheduler _amazonScheduler;

        public ScheduleClient(IOptions<Settings> settings, ILogger<ScheduleClient> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            _amazonScheduler = new AmazonSchedulerClient();
        }

        public async Task CreateGamesScheduleAsync(DateDetails dateDetails, DateTime firstStart, DateTime lastStart, CancellationToken cancellationToken)
        {
            var dateRequest = GenerateRequest(dateDetails);
            dateRequest.GetTeamStates = false;
            var requst = new CreateScheduleRequest
            {
                StartDate = firstStart,
                EndDate = lastStart.AddMinutes(_settings.MinutesAfterLastGameStarts),
                Target = new Target 
                { 
                    Arn = _settings.StatsLambdaARN, 
                    Input = dateRequest.Serialize(),
                    RoleArn = _settings.RoleARN
                },
                Name = $"{SchedulePrefix(dateDetails)}_gameStats",
                ScheduleExpression = $"rate({_settings.MinutesBetweenRuns} Minutes)",
                ScheduleExpressionTimezone = "UTC",
                FlexibleTimeWindow = new FlexibleTimeWindow { Mode = FlexibleTimeWindowMode.OFF }
            };
            try
            {
                var response = await _amazonScheduler.CreateScheduleAsync(requst, cancellationToken);
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"Failed to create stats schedule, status: {response.HttpStatusCode})");
                }
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"Failed to create stats schedule");
            }
        }

        public async Task CreateStateScheduleAsync(DateDetails dateDetails, CancellationToken cancellationToken)
        {
            var date = DateTime.Parse(dateDetails.GameId);
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            date = date.Date.AddHours(15);
            var dateRequest = GenerateRequest(dateDetails);
            dateRequest.GetTeamStates = true;
            var requst = new CreateScheduleRequest
            {
                Target = new Target
                {
                    Arn = _settings.StatsLambdaARN,
                    Input = dateRequest.Serialize(),
                    RoleArn = _settings.RoleARN
                },
                Name = $"{SchedulePrefix(dateDetails)}_gameState",
                ScheduleExpression = $"at({date.ToString("yyyy-MM-ddTHH:mm:ss")})",
                ScheduleExpressionTimezone = "UTC",
                FlexibleTimeWindow = new FlexibleTimeWindow { Mode = FlexibleTimeWindowMode.FLEXIBLE, MaximumWindowInMinutes = 30 }
            };
            try
            {
                var response = await _amazonScheduler.CreateScheduleAsync(requst, cancellationToken);
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"Failed to create state schedule, status: {response.HttpStatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create state schedule");
            }
        }

        public async Task CreateDayAfterScheduleAsync(DateDetails dateDetails, CancellationToken cancellationToken)
        {
            var date = DateTime.Parse(dateDetails.GameId);
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            date = date.AddDays(1);
            date = date.Date.AddHours(15);
            var dateRequest = GenerateRequest(dateDetails);
            dateRequest.GetTeamStates = false;
            var requst = new CreateScheduleRequest
            {
                Target = new Target
                {
                    Arn = _settings.StatsLambdaARN,
                    Input = dateRequest.Serialize(),
                    RoleArn = _settings.RoleARN
                },
                Name = $"{SchedulePrefix(dateDetails)}_final",
                ScheduleExpression = $"at({date.ToString("yyyy-MM-ddTHH:mm:ss")})",
                ScheduleExpressionTimezone = "UTC",
                FlexibleTimeWindow = new FlexibleTimeWindow { Mode = FlexibleTimeWindowMode.FLEXIBLE, MaximumWindowInMinutes = 30 }
            };
            try
            {
                var response = await _amazonScheduler.CreateScheduleAsync(requst, cancellationToken);
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"Failed to create final stats schedule, status: {response.HttpStatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create final stats schedule");
            }
        }

        private DateRequest GenerateRequest(DateDetails dateDetails)
        {
            return new DateRequest
            {
                SeasonId = dateDetails.SeasonId,
                GameDateId = dateDetails.GameId,
                IsPlayoffs = dateDetails.IsPlayoffs
            };
        }

        private string SchedulePrefix(DateDetails dateDetails)
        {
            var playoffText = dateDetails.IsPlayoffs ? "playoffs" : "regularSeason";
            return $"{dateDetails.GameId}_{playoffText}";
        }
    }
}
