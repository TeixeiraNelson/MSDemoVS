using MassTransit;
using MicroserviceEventlogs.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MicroserviceEventlogs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventLogsController : ControllerBase
    {
        private static readonly List<EventLog> BaseLogs = new List<EventLog>() {
            new EventLog(1, "AUTHENTIC", DateTime.Now),
            new EventLog(2,"SUSPECT", DateTime.Now),
            new EventLog(3,"AUTHENTIC", DateTime.Now)
        }; // demo inmemory eventlogs

        private readonly IBus _bus;
        public EventLogsController(IBus bus)
        {
            _bus = bus;
        }

        // GET: api/<EventLogsController>
        [HttpGet]
        public IEnumerable<EventLog> Get()
        {
            return BaseLogs;
        }

        // GET api/<EventLogsController>/5
        [HttpGet("{id}")]
        public EventLog Get(int id)
        {
            return BaseLogs.Where(b => b.ID == id).SingleOrDefault();
        }

        // POST api/<EventLogsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EventLog newEventLog)
        {
            if (newEventLog != null)
            {
                newEventLog.Time = DateTime.Now;
                BaseLogs.Add(newEventLog);
                Uri uri = new Uri("rabbitmq://localhost/eventLogsQueue");

                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(newEventLog);
                
                return Ok();
            }
            return BadRequest();
        }

        // PUT api/<EventLogsController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] EventLog newEventLog)
        {
            EventLog eventlogToReplace = BaseLogs.First(b => b.ID == id);
            var index = BaseLogs.IndexOf(eventlogToReplace);

            if (index != -1)
            {
                BaseLogs[index] = eventlogToReplace;
                return Ok(eventlogToReplace);
            }
            return NotFound(); 
        }

        // DELETE api/<EventLogsController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            EventLog eventlogToDelete = BaseLogs.SingleOrDefault(b => b.ID == id);

            if (eventlogToDelete != null)
            {
                BaseLogs.Remove(eventlogToDelete);
                return Ok();
            }
            return NotFound();
        }
    }
}
