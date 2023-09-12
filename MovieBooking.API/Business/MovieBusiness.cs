using AutoMapper;
using MovieBooking.API.Interfaces.IBusiness;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models.DTO;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Business
{
    public class MovieBusiness : IMovieBusiness
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieBusiness> _logger;

        public MovieBusiness(IMovieRepository movieRepository, ILogger<MovieBusiness> logger)
        {
            _movieRepository = movieRepository;
            _logger = logger;
        }
        public async Task<List<TotalMovieDto>> GetMovies()
        {
            _logger.LogInformation("Get all movies : movie Business");

            List<TotalMovieDto> moviesView;
            try
            {

                var movieList = new List<TotalMovieDto>();
                var movies = await _movieRepository.GetMovies();
                if (movies.Count > 0)
                {
                    foreach (var movie in movies)
                    {
                        var movieResponse =
                            new TotalMovieDto
                            {
                                Name = movie.MovieName,
                                NumberOfTickets = movie.NumberOfTickets,
                                TicketStatus = movie.TicketStatus,
                                ImageUrl = movie.ImageUrl
                            };
                        movieList.Add(movieResponse);
                    }
                    return movieList;
                }
                return null;


            }
            catch(Exception)
            {
                moviesView = new();
            }

            return moviesView;
        }

        public async Task<List<MovieDto>> GetMovieStatus()
        {
            _logger.LogInformation("Get all movies : movie Business");

            List<MovieDto> moviesView;
            try
            {

                var movieList = new List<MovieDto>();
                var movies = await _movieRepository.GetMovieStatusDB();
                if (movies.Count > 0)
                {
                    foreach (var movie in movies)
                    {
                        var movieResponse =
                            new MovieDto
                            {
                                Name = movie.MovieName,
                                TheatreName=movie.TheatreName,
                                NumberOfTickets = movie.NumberOfTickets,
                                TicketStatus = movie.TicketStatus,
                                ImageUrl = movie.ImageUrl
                            };
                        movieList.Add(movieResponse);
                    }
                    return movieList;
                }
                return null;


            }
            catch (Exception)
            {
                moviesView = new();
            }

            return moviesView;
        }


        public async Task<List<TotalMovieDto>> SearchMovie(string movieName)
        {
            _logger.LogInformation("Get movie by name : movie Business");
         
                var moviesModel = await _movieRepository.SearchMovie(movieName);

                var movieList = new List<TotalMovieDto>();

                if (moviesModel.Count > 0)
                {
                    foreach (var movie in moviesModel)
                    {
                        var movieResponse =
                            new TotalMovieDto
                            {
                                Name = movie.MovieName,
                                NumberOfTickets = movie.NumberOfTickets,
                                TicketStatus = movie.TicketStatus,
                                ImageUrl = movie.ImageUrl
                            };
                        movieList.Add(movieResponse);
                    }
                    return movieList;
                }
            
           return null;
        }

        public async Task<MovieResponse> AddMovieAsync(MovieDto movie)
        {
            try
            {
                _logger.LogInformation("Add movie : admin service");

                var movieRequest = new MovieStatus
                {
                    MovieName = movie.Name,
                    TheatreName = movie.TheatreName,
                    NumberOfTickets = movie.NumberOfTickets,
                    TicketStatus = movie.TicketStatus,
                    ImageUrl= movie.ImageUrl
                };
                await _movieRepository.AddMovieRepoAsync(movieRequest);
                return new MovieResponse
                {
                    Message = "Movie added succeessfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Add movie failed");
                return new MovieResponse
                {
                    Message = ex.Message,
                    Success = false
                };
            }

        }

        public async Task<MovieResponse> DeleteMovieAsync(string moviename)
        {
            try
            {
                _logger.LogInformation("Delete movie : movie Business");


                var result = await _movieRepository.GetMovieRepoAsync(moviename);
                foreach (var movie in result)
                {
                    await _movieRepository.DeleteMovieRepoAsync(movie.Id);
                    await _movieRepository.DeleteTotalMovieRepoAsync(movie.MovieName);
                    await _movieRepository.DeleteInitialMovieCountRepoAsync(movie.MovieName);
                    return new MovieResponse
                    {
                        Message = "Deleted movie successfully",
                        Success = true
                    };
                }
                return new MovieResponse
                {
                    Message = "Movie deletion failed",
                    Success = false
                };
            }
            catch (Exception ex)
            {

                return new MovieResponse
                {
                    Message = ex.Message,
                    Success = false
                };
            }
        }

        public async Task<MovieStatus> UpdateMovieStatus(string moviename, string status)
        {
           var movieStatus= await _movieRepository.UpdateMovieStatus(moviename, status);

            if(movieStatus is null)
            {
                return null;
            }
            else
            {
                return movieStatus;
            }
        }

       public async Task<int> AvailableTicketCount(string moviename, string theatreName)
        {

            var count = await _movieRepository.ticketcount(moviename, theatreName);
            if(count!=0)
            {
                return count;
            }
            else
            {
                return 0;
            }
        }

        public async Task<InitialMovieTicket> TicketAndStatus(string moviename, string theatreName)
        {

            var moviedata = await _movieRepository.ticketandstatus(moviename, theatreName);

            if(moviedata is not null)
            {
                return moviedata;
            }
            else
            {
                return null;
            }
        }

        public async Task<MovieResponse> UpdateMovieAsync(InitialMovieTicketDto movie)
        {
            try
            {
               

                var movieRequest = new InitialMovieTicket
                {
                    MovieName = movie.Name,
                    TheatreName = movie.TheatreName,
                    NumberOfTickets = movie.NumberOfTickets,
                    TicketStatus = movie.TicketStatus
                };

                var result = await _movieRepository.BookingAvailableRepoAsync(movie.Name,movie.TheatreName);
                var InitialMovieCountdata = await _movieRepository.InitialTicketCountAvailableRepoAsync(movie.Name, movie.TheatreName);
                
                if (result.Count > 0)
                {
                    foreach (var movies in result)
                    {
                        movies.NumberOfTickets= movie.NumberOfTickets;
                        movies.TicketStatus= movie.TicketStatus;

                        await _movieRepository.UpdateMovieRepoAsync(movies);
                    }
                }

                if (InitialMovieCountdata.Count > 0)
                {
                    foreach (var movies in InitialMovieCountdata)
                    {
                        movies.NumberOfTickets = movie.NumberOfTickets;
                        movies.TicketStatus = movie.TicketStatus;

                        await _movieRepository.UpdateInitialMovieCountRepoAsync(movies);
                    }
                }

                return new MovieResponse
                {
                    Message = "Updated succeessfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Update movie failed");
                return new MovieResponse
                {
                    Message = ex.Message,
                    Success = false
                };
            }

        }

       /*public async Task<List<TicketBookedCountResponse>> TotalTicketCountAsync(string moviename)
        {

        }*/



    }
}
