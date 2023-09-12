using Kafka_Consumer.Model;

namespace Kafka_Consumer.IRepository
{
    public interface IAdminRepository
    {
        Task<List<Ticket>> BookingCountRepoAsync(string movieName);

        Task<List<MovieStatus>> BookingAvailableRepoAsync(string movieName);

        Task UpdateTicketStatus(MovieStatus movie);

        Task<List<MovieStatus>> GetMovieRepoAsync(string movieName);
    }
}
