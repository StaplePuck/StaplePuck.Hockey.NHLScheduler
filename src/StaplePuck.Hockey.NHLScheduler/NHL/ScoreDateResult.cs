using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaplePuck.Hockey.NHLScheduler.NHL
{
    public class ScoreDateResult
    {
        public string prevDate { get; set; } = string.Empty;
        public string currentDate { get; set; } = string.Empty;
        public string nextDate { get; set; } = string.Empty;
        public Game[] games { get; set; } = new Game[0];

        public class Game
        {
            public int id { get; set; }
            public int season { get; set; }
            public int gameType { get; set; }
            public string gameState { get; set; } = string.Empty;
            public DateTime startTimeUTC { get; set; }
        }
    }
}
