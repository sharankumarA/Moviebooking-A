using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models.Appsettings;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IOptions<MongoDbConfig> _mongoDbConfig;
        private readonly IMongoCollection<MovieStatus> _movieCollection;
        private readonly IMongoCollection<TotalBookedMovies> _totalmovieCollection;
        private readonly IMongoCollection<InitialMovieTicket> _InitialMovieTicketCollection;
        private readonly ILogger<MovieRepository> _logger;
        public MovieRepository(IOptions<MongoDbConfig> mongoDbConfig, IMongoClient mongoClient, ILogger<MovieRepository> logger)
        {
            _mongoDbConfig = mongoDbConfig;

            var database = mongoClient.GetDatabase(_mongoDbConfig.Value.DatabaseName);
            _movieCollection = database.GetCollection<MovieStatus>(_mongoDbConfig.Value.MovieStatusCollectionName);
            _InitialMovieTicketCollection = database.GetCollection<InitialMovieTicket>(_mongoDbConfig.Value.InitialMovieTicketCollectionName);
            _totalmovieCollection = database.GetCollection<TotalBookedMovies>(_mongoDbConfig.Value.TotalBookedMoviesCollectionName);
            _logger = logger;
        }
        public async Task<List<TotalBookedMovies>> GetMovies()
        {
            _logger.LogInformation("Get movie list from movie collection : movie repository");

            return await _totalmovieCollection.Find(_ => true).ToListAsync();
        }
        public async Task<List<MovieStatus>> GetMovieStatusDB()
        {
            _logger.LogInformation("Get movie list from movie collection : movie repository");

            return await _movieCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<TotalBookedMovies>> SearchMovie(string movieName)
        {
            _logger.LogInformation("Get movie list by name from movie collection : movie repository");

            return await _totalmovieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).ToListAsync();
        }
        public async Task<bool> checkifmovieexist(string movieName)
        {
            _logger.LogInformation("Get movie list by name from movie collection : movie repository");

            var result = await _totalmovieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).FirstOrDefaultAsync();
            return (result != null) ? true : false;
        }


        public async Task AddMovieRepoAsync(MovieStatus movie)
        {
            _logger.LogInformation("Add movie to movie collection : admin repository");

            await _movieCollection.InsertOneAsync(movie);

            var InitialTicketCount = new InitialMovieTicket
            {
                MovieName = movie.MovieName,
                TheatreName = movie.TheatreName,
                NumberOfTickets = movie.NumberOfTickets,
                TicketStatus = movie.TicketStatus,
            };
            await _InitialMovieTicketCollection.InsertOneAsync(InitialTicketCount);
            var bookingData = new TotalBookedMovies
            {
                MovieName = movie.MovieName,
                NumberOfTickets = movie.NumberOfTickets,
                TicketStatus = movie.TicketStatus,
                ImageUrl = movie.ImageUrl
            };
            var result = await checkifmovieexist(movie.MovieName);
            if (result)
            {
                var moviename = movie.MovieName;
                var previousticket = await _totalmovieCollection.Find(x => x.MovieName.ToLower().Contains(moviename.ToLower())).FirstOrDefaultAsync();
                var ticketCount = movie.NumberOfTickets;
                previousticket.NumberOfTickets += ticketCount;

                await _totalmovieCollection.ReplaceOneAsync(x => x.Id == previousticket.Id, previousticket);
            }
            else
            {
                await _totalmovieCollection.InsertOneAsync(bookingData);
            }
        }

        public async Task DeleteMovieRepoAsync(string id)
        {
            _logger.LogInformation("Delete movie from movie collection : movie repository");

            await _movieCollection.DeleteOneAsync(x => x.Id == id);

        }

        public async Task DeleteTotalMovieRepoAsync(string moviename)
        {
            await _totalmovieCollection.DeleteOneAsync(x => x.MovieName == moviename);
        }
        public async Task DeleteInitialMovieCountRepoAsync(string moviename)
        {
            await _InitialMovieTicketCollection.DeleteOneAsync(x => x.MovieName == moviename);
        }

        
        public async Task<List<MovieStatus>> GetMovieRepoAsync(string movieName)
        {
            _logger.LogInformation("Get movie list from movie collection : movie repository");

            return await _movieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).ToListAsync();
        }

        public async Task<MovieStatus> UpdateMovieStatus(string movieName, string Status)
        {
            var result = await _movieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).FirstOrDefaultAsync(); ;
            result.TicketStatus = Status;
            return result;
        }
        public async Task<int> ticketcount(string moviename, string theatreName)
        {
            var filter = Builders<InitialMovieTicket>.Filter.Eq(t => t.MovieName, moviename) &
                      Builders<InitialMovieTicket>.Filter.Eq(t => t.TheatreName, theatreName);

            var movieTicket = await _InitialMovieTicketCollection.Find(filter).FirstOrDefaultAsync();

            return movieTicket.NumberOfTickets;

        }

        public async Task<InitialMovieTicket> ticketandstatus(string moviename, string theatreName)
        {
            var filter = Builders<InitialMovieTicket>.Filter.Eq(x => x.MovieName, moviename) &
                         Builders<InitialMovieTicket>.Filter.Eq(x => x.TheatreName, theatreName);

            var matchingData = await _InitialMovieTicketCollection.Find(filter).FirstOrDefaultAsync();

            return matchingData;
        }

        public async Task<List<MovieStatus>> BookingAvailableRepoAsync(string movieName, string theatreName)
        {
            var filter = Builders<MovieStatus>.Filter.Eq(x => x.MovieName, movieName) &
                 Builders<MovieStatus>.Filter.Eq(x => x.TheatreName, theatreName);

            var matchingMovies = await _movieCollection.Find(filter).ToListAsync();

            return matchingMovies;
        }
        public async Task<List<InitialMovieTicket>> InitialTicketCountAvailableRepoAsync(string movieName, string theatreName)
        {
            var filter = Builders<InitialMovieTicket>.Filter.Eq(x => x.MovieName, movieName) &
                 Builders<InitialMovieTicket>.Filter.Eq(x => x.TheatreName, theatreName);

            var matchingMovies = await _InitialMovieTicketCollection.Find(filter).ToListAsync();

            return matchingMovies;
        }
        public async Task UpdateMovieRepoAsync(MovieStatus movie)
        {

            await _movieCollection.ReplaceOneAsync(x => x.Id == movie.Id, movie);
        }

        public async Task UpdateInitialMovieCountRepoAsync(InitialMovieTicket movie)
        {

            await _InitialMovieTicketCollection.ReplaceOneAsync(x => x.Id == movie.Id, movie);

           // await _movieCollection.ReplaceOneAsync(x => x.Id == movie.Id, movie);

        }


    }
}
