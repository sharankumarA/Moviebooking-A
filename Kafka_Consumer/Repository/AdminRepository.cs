using Kafka_Consumer.IRepository;
using Kafka_Consumer.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Kafka_Consumer.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IMongoCollection<Ticket> _ticketCollection;
        private readonly IMongoCollection<MovieStatus> _movieCollection;

        private readonly ILogger<AdminRepository> _logger;
        public AdminRepository(IOptions<MovieTicketDatabaseSettings> movieTicketDatabaseSettings, ILogger<AdminRepository> logger)
        {
            var mongoClient = new MongoClient(movieTicketDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(movieTicketDatabaseSettings.Value.DatabaseName);
            _ticketCollection = mongoDatabase.GetCollection<Ticket>(movieTicketDatabaseSettings.Value.TicketCollectioName);
            _movieCollection = mongoDatabase.GetCollection<MovieStatus>(movieTicketDatabaseSettings.Value.MovieCollectionName);
            _logger = logger;
        }

        public async Task<List<MovieStatus>> BookingAvailableRepoAsync(string movieName)
        {
            _logger.LogInformation("Get list of movie available from movie collection: admin kafka repository");

            return await _movieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).ToListAsync();
        }

        public async Task<List<Ticket>> BookingCountRepoAsync(string movieName)
        {
            _logger.LogInformation("Get list of tickets booked from ticket collection: admin kafka repository");

            return await _ticketCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).ToListAsync();
        }

        public async Task UpdateTicketStatus(MovieStatus movie)
        {
            _logger.LogInformation("Update ticket status in ticket collection : admin kafka repository");

            await _movieCollection.ReplaceOneAsync(x => x.Id == movie.Id, movie);
        }

        public async Task<List<MovieStatus>> GetMovieRepoAsync(string movieName)
        {
            _logger.LogInformation("Get list of movie by name from movie collection : admin kafka repository");

            return await _movieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).ToListAsync();
        }
    }
}
