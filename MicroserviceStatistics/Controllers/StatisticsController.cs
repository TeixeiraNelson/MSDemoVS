using MicroserviceStatistics.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroserviceStatistics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        public static readonly EventLogsStats BaseStats = new EventLogsStats(2,1); // demo inmemory eventlogs

        // GET: api/<EventLogsController>
        [HttpGet]
        public EventLogsStats Get()
        {
            return BaseStats;
        }
    }
}
