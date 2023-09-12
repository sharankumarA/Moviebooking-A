using MovieBooking.API.Models.DTO;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Interfaces.IBusiness
{
    public interface IMovieBusiness
    {
        public Task<List<TotalMovieDto>> GetMovies();
        public Task<List<TotalMovieDto>> SearchMovie(string movieName);
        Task<MovieResponse> AddMovieAsync(MovieDto movie);
        Task<MovieResponse> DeleteMovieAsync(string moviename);
        Task<MovieStatus> UpdateMovieStatus(string moviename, string status);
        Task<int> AvailableTicketCount(string moviename, string theatreName);
        Task<InitialMovieTicket> TicketAndStatus(string moviename, string theatreName);
        Task<MovieResponse> UpdateMovieAsync(InitialMovieTicketDto movie);
        // public Task<List<TicketBookedCountResponse>> TotalTicketCountAsync();
        public Task<List<MovieDto>> GetMovieStatus();



    }
}
