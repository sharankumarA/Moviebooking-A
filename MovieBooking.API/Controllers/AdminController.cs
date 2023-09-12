using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MovieBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly string bootstrapServers = "localhost:9092";
        private readonly string topic = "ticketbooking";
        private readonly ILogger<AdminController> _logger;
        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("ticket-booked/{movieName}")]
        
        public IActionResult GetNumberOfTicketsBooked(string movieName)
        {
            _logger.LogInformation("Get number of tickets booked : admin controller");
            _logger.LogDebug($"moviename: {movieName}");

            string message = JsonSerializer.Serialize(movieName);
            return Ok(SendToKafka(topic, message));
        }

        [HttpGet]
        [Route("ticket-available/{movieName}")]
        
        public IActionResult GetNumberOfTicketsAvailable(string movieName)
        {
            _logger.LogInformation("Get number of tickets available : admin controller");
            _logger.LogDebug($"moviename: {movieName}");

            string message = JsonSerializer.Serialize(movieName);
            return Ok(SendToKafka(topic, message));
        }

        [HttpPut]
        [Route("{moviename}/update")]
        
        public IActionResult UpdateTicketStatus(string moviename)
        {
            _logger.LogInformation("Update ticket status : admin controller");
            _logger.LogDebug($"moviename: {moviename}");

            string message = JsonSerializer.Serialize(moviename);
            return Ok(SendToKafka(topic, message));
        }

       
        private Object SendToKafka(string topic, string message)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = Dns.GetHostName()
            };

            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    _logger.LogInformation("Producer kafka initiated : admin repository");

                    var result = producer.ProduceAsync
                    (topic, new Message<Null, string>
                    {
                        Value = message
                    }).GetAwaiter().GetResult();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while connecting to Kafka");
                Console.WriteLine($"Error occured: {ex.Message}");
            }
            return null;
        }
    
}
}
