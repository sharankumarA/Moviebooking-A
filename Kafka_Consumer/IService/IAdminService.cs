using Kafka_Consumer.Dto;

namespace Kafka_Consumer.IService
{
    public interface IAdminService
    {
        Task<List<TicketBookedCountResponse>> BookingCountAsync(string movieName);

        Task<List<TicketBookedCountResponse>> BookingAvailableAsync(string movieName);

        Task UpdateTicketStatus(string moviename);
    }
}
