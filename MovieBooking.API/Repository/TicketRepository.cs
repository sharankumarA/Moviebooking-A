using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models.Appsettings;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Repository
{
    public class TicketRepository:ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _ticketCollection;
        private readonly IMongoCollection<MovieStatus> _movieCollection;
        private readonly IMongoCollection<TotalBookedMovies> _totalmovieCollection;
        private readonly IMongoCollection<TotalTicketCount> _totalticketcountCollection;


        private readonly ILogger<TicketRepository> _logger;
        public TicketRepository(IOptions<MongoDbConfig> movieTicketDatabaseSettings, ILogger<TicketRepository> logger)
        {
            var mongoClient = new MongoClient(movieTicketDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(movieTicketDatabaseSettings.Value.DatabaseName);
            _ticketCollection = mongoDatabase.GetCollection<Ticket>(movieTicketDatabaseSettings.Value.TicketCollectionName);
            _movieCollection = mongoDatabase.GetCollection<MovieStatus>(movieTicketDatabaseSettings.Value.MovieStatusCollectionName);
            _totalmovieCollection = mongoDatabase.GetCollection<TotalBookedMovies>(movieTicketDatabaseSettings.Value.TotalBookedMoviesCollectionName);
            _totalticketcountCollection = mongoDatabase.GetCollection<TotalTicketCount>(movieTicketDatabaseSettings.Value.TotalTicketCountCollectionName);
            _logger = logger;
        }

 

        public async Task BookMovieRepoAsync(Ticket ticket)
        {
            _logger.LogInformation("Add ticket to ticket collection : ticket repository");

            var Movieticketcount = new TotalTicketCount
            {
                MovieName = ticket.MovieName,
                NumberOfTickets = ticket.NumberOfTickets,
                ImageUrl = string.Empty
            };

                   var result = await _totalticketcountCollection
                   .Find(x => x.MovieName.ToLower() == ticket.MovieName.ToLower())
                   .FirstOrDefaultAsync();

            if (result != null)
            {
                result.NumberOfTickets += ticket.NumberOfTickets;
                await _totalticketcountCollection.ReplaceOneAsync(x => x.Id == result.Id, result);
            }
            else
            {
                // Fetch the MovieStatus entry for the respective movie
                var movieStatus = await _movieCollection
                    .Find(x => x.MovieName.ToLower() == ticket.MovieName.ToLower())
                    .FirstOrDefaultAsync();

                if (movieStatus != null)
                {
                    // Assign the ImageUrl from MovieStatus to TotalTicketCount
                    Movieticketcount.ImageUrl = movieStatus.ImageUrl;
                }

                await _totalticketcountCollection.InsertOneAsync(Movieticketcount);
            }

            await _ticketCollection.InsertOneAsync(ticket);
        }

        public async Task<bool> CheckIfMovieForTicketExists(string movieName)
        {
            _logger.LogInformation("Check if movie exists in movie collection : ticket repository");

            var result = await _totalticketcountCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).FirstOrDefaultAsync();
            return (result != null) ? true : false;
        }
        public async Task<bool> CheckIfMovieExists(string movieName)
        {
            _logger.LogInformation("Check if movie exists in movie collection : ticket repository");

            var result = await _movieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).FirstOrDefaultAsync();
            return (result != null) ? true : false;
        }

        public async Task<MovieStatus> GetMovieAgainstTheatreAsync(string theatreName, string movieName)
        {

            return await _movieCollection.Find(x => x.TheatreName.ToLower().Contains(theatreName.ToLower()) && x.MovieName.ToLower().Contains(movieName.ToLower())).FirstOrDefaultAsync();
        }
        public async Task<TotalBookedMovies> GetTotalTicketsAsync(string movieName)
        {

            return await _totalmovieCollection.Find(x =>x.MovieName.ToLower().Contains(movieName.ToLower())).FirstOrDefaultAsync();
        }

        public async Task UpdateTicketCount(MovieStatus movie)
        {
         
           await _movieCollection.ReplaceOneAsync(x => x.Id == movie.Id, movie);
        }
        public async Task UpdateTotalTicketCount(TotalBookedMovies totalMovie)
        {

            await _totalmovieCollection.ReplaceOneAsync(x => x.Id == totalMovie.Id, totalMovie);
        }

        public async Task<List<Ticket>> getcartdetails(string username)
        {
            var filter = Builders<Ticket>.Filter.Eq("UserName", username);
            var ticket = await _ticketCollection.Find(filter).ToListAsync();
            return ticket;
        }
        public async Task<List<MovieStatus>> GetTheatreName(string moviename)
        {
            var name = Builders<MovieStatus>.Filter.Eq(m => m.MovieName, moviename);
            var moviesWithGivenName = await _movieCollection.Find(name).ToListAsync();
            // Extract the theatre names from the movies found
            

            return moviesWithGivenName;
        }
        public async Task<List<int>> bookedcount(string moviename, string theatreName)
        {
            var filter = Builders<Ticket>.Filter.And(
             Builders<Ticket>.Filter.Eq("MovieName", moviename),
             Builders<Ticket>.Filter.Eq("TheatreName", theatreName)
            );

            var projection = Builders<Ticket>.Projection.Include("SeatNumber").Exclude("_id");
            var result = await _ticketCollection.Find(filter).Project(projection).ToListAsync();

            // Convert the list of BsonDocument to a list of integers (seat numbers)
           var seatNumbers = new List<int>();
            foreach (var doc in result)
            {
                var seatNumberValue = doc["SeatNumber"];
                if (seatNumberValue.IsInt32)
                {
                    seatNumbers.Add(seatNumberValue.AsInt32);
                }
                else if (seatNumberValue.IsBsonArray)
                {
                    var seatNumberArray = seatNumberValue.AsBsonArray;
                    foreach (var item in seatNumberArray)
                    {
                        if (item.IsInt32)
                        {
                            seatNumbers.Add(item.AsInt32);
                        }
                    }
                }
            }

            return seatNumbers;
        }
        public async Task<List<TotalTicketCount>> getallticketcount()
        {
            return await _totalticketcountCollection.Find(_ => true).ToListAsync();
        }
    }
}
