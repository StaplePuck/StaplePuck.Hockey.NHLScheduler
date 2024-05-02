using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaplePuck.Hockey.NHLScheduler
{
    public class DateRequest
    {
        public string SeasonId { get; set; } = string.Empty;
        public string GameDateId { get; set; } = string.Empty;
        public bool GetTeamStates { get; set; }
        public bool IsPlayoffs { get; set; }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
