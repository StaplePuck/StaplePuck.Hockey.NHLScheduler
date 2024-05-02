using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaplePuck.Hockey.NHLScheduler
{
    public class Settings
    {
        public string ApiUrlRoot { get; set; } = string.Empty;
        public string StatsLambdaARN { get; set; } = string.Empty;
        public string RoleARN { get; set; } = string.Empty;
        public int MinutesBetweenRuns { get; set; }
        public int MinutesAfterLastGameStarts { get; set; }

    }
}
