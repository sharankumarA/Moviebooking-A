
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Interfaces.IRepository
{
    public interface ITicketRepository
    {
        Task BookMovieRepoAsync(Ticket ticket);
        Task<bool> CheckIfMovieExists(string movieName);
        Task<bool> CheckIfMovieForTicketExists(string movieName);
        Task<MovieStatus> GetMovieAgainstTheatreAsync(string theatreName, string movieName);
        Task<TotalBookedMovies> GetTotalTicketsAsync(string movieName);
        Task UpdateTicketCount(MovieStatus movie);
        Task UpdateTotalTicketCount(TotalBookedMovies totalMovie);

        Task<List<Ticket>> getcartdetails(string username);
        public Task<List<MovieStatus>> GetTheatreName(string moviename);
        public Task<List<int>> bookedcount(string moviename, string theatreName);
        public Task<List<TotalTicketCount>> getallticketcount();




    }
}
