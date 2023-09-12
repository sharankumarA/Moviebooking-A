using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Interfaces.IRepository
{
    public interface IMovieRepository
    {
        public Task<List<TotalBookedMovies>> GetMovies();
        public Task<List<TotalBookedMovies>> SearchMovie(string movieName);
        public Task AddMovieRepoAsync(MovieStatus movie);
        public Task DeleteMovieRepoAsync(string id);
        public Task DeleteTotalMovieRepoAsync(string moviename);
        public Task DeleteInitialMovieCountRepoAsync(string moviename);
        public Task<List<MovieStatus>> GetMovieRepoAsync(string movieName);
        public Task<MovieStatus> UpdateMovieStatus(string movieName, string movieStatus);
        public Task<bool> checkifmovieexist(string movieName);
        public Task<int> ticketcount(string moviename, string theatreName);
        public Task<InitialMovieTicket> ticketandstatus(string moviename, string theatreName);
        
        public Task<List<MovieStatus>> BookingAvailableRepoAsync(string movieName, string theatreName);
        public Task<List<InitialMovieTicket>> InitialTicketCountAvailableRepoAsync(string movieName, string theatreName);
        public Task UpdateMovieRepoAsync(MovieStatus movie);
        public Task UpdateInitialMovieCountRepoAsync(InitialMovieTicket movie);
        public  Task<List<MovieStatus>> GetMovieStatusDB();



    }
}
