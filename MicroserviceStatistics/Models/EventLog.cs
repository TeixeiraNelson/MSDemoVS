using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceEventlogs.Models
{
    public class EventLog
    {
        public EventLog(int ID, string Type, DateTime Time)
        {
            this.ID = ID;
            this.Type = Type;
            this.Time = Time;
        }

        public int ID { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
    }
}
