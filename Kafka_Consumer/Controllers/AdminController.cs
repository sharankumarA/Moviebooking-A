using Kafka_Consumer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kafka_Consumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        private readonly ILogger<AdminController> _logger;
        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet]
        [Route("ticket-booked/{movieName}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetNumberOfTicketsBooked(string movieName)
        {
            _logger.LogInformation("Get number of tickets booked : admin kafka controller");
            _logger.LogDebug($"moviename: {movieName}");

            var result = await _adminService.BookingCountAsync(movieName);
            return (result.Count != 0) ? Ok(result) : BadRequest("No bookings");

        }

        [HttpGet]
        [Route("ticket-available/{movieName}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetNumberOfTicketsAvailable(string movieName)
        {
            _logger.LogInformation("Get number of tickets available : admin kafka controller");
            _logger.LogDebug($"moviename: {movieName}");

            var result = await _adminService.BookingAvailableAsync(movieName);
            return (result.Count != 0) ? Ok(result) : BadRequest();
        }

        [HttpPut]
        [Route("{moviename}/update")]
        public async Task<IActionResult> UpdateTicketStatus(string moviename)
        {
            _logger.LogInformation("Update ticket status : admin kafka controller");
            _logger.LogDebug($"moviename: {moviename}");

            await _adminService.UpdateTicketStatus(moviename);
            var result = await _adminService.BookingAvailableAsync(moviename);
            return (result.Count != 0) ? Ok(result) : BadRequest();
        }

    }
}
