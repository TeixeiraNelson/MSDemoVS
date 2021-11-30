using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceStatistics.Models
{
    public class EventLogsStats
    {
        public EventLogsStats(int authenticCount, int suspectCount)
        {
            this.AuthenticCount = authenticCount;
            this.SuspectCount = suspectCount;
        }

        public int AuthenticCount { get; set; }
        public int SuspectCount { get; set; }
    }
}
