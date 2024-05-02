using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaplePuck.Hockey.NHLScheduler.Schedule
{
    public class DateDetails
    {
        public string SeasonId { get; init; } = string.Empty;
        public string GameId { get; init; } = string.Empty;
        public bool IsPlayoffs { get; init; }
    }
}
