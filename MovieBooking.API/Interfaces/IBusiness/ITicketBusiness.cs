using MovieBooking.API.Models.DTO;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Interfaces.IBusiness
{
    public interface ITicketBusiness
    {
        Task<TicketBookingResponse> BookMovieAsync(string moviename, TicketBookRequest ticket);

        Task TicketCountAsync(string moviename, TicketBookRequest ticket);
        Task<List<TicketDto>> CartDetails(string username);
        Task<List<string>> theatreName(string moviename);
        Task<List<int>> BookedTicketCount(string moviename, string theatreName);

        Task<List<TotalTicketCount>> GetTickets();
    }
}
