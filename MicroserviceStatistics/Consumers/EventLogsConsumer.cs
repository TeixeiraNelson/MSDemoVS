using MassTransit;
using MicroserviceEventlogs.Models;
using MicroserviceStatistics.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceStatistics.Consumers
{
    public class EventLogsConsumer : IConsumer<EventLog>
    {
        public async Task Consume(ConsumeContext<EventLog> context)
        {
            var data = context.Message;
            //Validate the Ticket Data
            //update DB
            if (data.Type == "AUTHENTIC")
                StatisticsController.BaseStats.AuthenticCount++;
            else
                StatisticsController.BaseStats.SuspectCount++;
        }
    }
}
