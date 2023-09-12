using Kafka_Consumer.Dto;
using Kafka_Consumer.IRepository;
using Kafka_Consumer.IService;

namespace Kafka_Consumer.Service
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        private readonly ILogger<AdminService> _logger;
        public AdminService(IAdminRepository adminRepository, ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _logger = logger;
        }

        public async Task<List<TicketBookedCountResponse>> BookingAvailableAsync(string movieName)
        {
            _logger.LogInformation("Get list of movie available : admin kafka service");

            var result = await _adminRepository.BookingAvailableRepoAsync(movieName);
            List<TicketBookedCountResponse> bookingList = new List<TicketBookedCountResponse>();
            if (result.Count > 0)
            {
                foreach (var value in result)
                {
                    var movies = new TicketBookedCountResponse
                    {
                        TheatreName = value.TheatreName,
                        TicketCount = value.NumberOfTickets,
                        Status = value.TicketStatus
                    };
                    bookingList.Add(movies);
                }
            }
            return bookingList;
        }

        public async Task<List<TicketBookedCountResponse>> BookingCountAsync(string movieName)
        {
            _logger.LogInformation("Get list of tickets booked : admin kafka service");

            var result = await _adminRepository.BookingCountRepoAsync(movieName);
            var ticketbooked = result.GroupBy(k => new { k.TheatreName }).Select(x => new TicketBookedCountResponse
            { TheatreName = x.Key.TheatreName, TicketCount = x.ToList().Sum(k => k.NumberOfTickets) }).ToList();
            return ticketbooked;
        }

        public async Task UpdateTicketStatus(string moviename)
        {
            _logger.LogInformation("Update ticket status : admin kafka service");

            var result = await _adminRepository.BookingAvailableRepoAsync(moviename);
            if (result.Count > 0)
            {
                foreach (var movie in result)
                {
                    if (movie.NumberOfTickets > 0)
                    {
                        movie.TicketStatus = "BOOK ASAP";
                    }
                    else
                    {
                        movie.TicketStatus = "SOLD OUT";
                    }
                    await _adminRepository.UpdateTicketStatus(movie);
                }
            }
        }
    }
}
