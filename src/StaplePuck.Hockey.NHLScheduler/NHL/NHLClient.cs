using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace StaplePuck.Hockey.NHLScheduler.NHL;
public class NHLClient : INHLClient
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly Settings _settings;
    private readonly ILogger _logger;

    public NHLClient(IOptions<Settings> options, ILogger<NHLClient> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public Task<ScoreDateResult?> GetGamesOnDateAsync(string dateId, CancellationToken cancellationToken)
    {
        // get games
        var url = $"{_settings.ApiUrlRoot}/v1/score/{dateId}";
        try
        {
            return _httpClient.GetFromJsonAsync<ScoreDateResult>(url, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get games for {dateId}");
        }

        return null;
    }
}

