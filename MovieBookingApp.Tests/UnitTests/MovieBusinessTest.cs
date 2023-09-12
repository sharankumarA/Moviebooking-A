using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using MovieBooking.API.Business;
using MovieBooking.API.Interfaces.IBusiness;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models.DTO;
using MovieBooking.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingApp.Tests.UnitTests
{
    [TestFixture]
   public class MovieBusinessTest
    {
        private IMovieBusiness _movieBusiness;
        private IFixture _fixture;

        private readonly Mock<ILogger<MovieBusiness>> _logger = new Mock<ILogger<MovieBusiness>>();

        private readonly Mock<IMovieRepository> _movieRepositoryMock = new Mock<IMovieRepository>();

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _movieBusiness = new MovieBusiness(_movieRepositoryMock.Object, _logger.Object);
        }

        [Test]
        public async Task GetAllMovies_Valid_ReturnsTrue()
        {
            List<MovieStatus> movies = new List<MovieStatus>()
            {
                new MovieStatus
                {
                    MovieName = "Sita",
                    TheatreName = "PVR",
                    TicketStatus = "BOOK ASAP",
                    NumberOfTickets = 100
                }
            };
            _movieRepositoryMock.Setup(m => m.GetMovies())
                .ReturnsAsync(movies);
            var result = await _movieBusiness.GetMovies();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetAllMovies_InValid_ReturnsFalse()
        {
            var movies = _fixture.Build<List<MovieStatus>>().Create();
            _movieRepositoryMock.Setup(m => m.GetMovies())
                .ReturnsAsync(movies);
            var result = await _movieBusiness.GetMovies();
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetMovieByName_Valid_ReturnsTrue()
        {
            List<MovieStatus> movies = new List<MovieStatus>()
            {
                new MovieStatus
                {
                    MovieName = "Sita",
                    TheatreName = "PVR",
                    TicketStatus = "BOOK ASAP",
                    NumberOfTickets = 100
                }
            };
            _movieRepositoryMock.Setup(m => m.SearchMovie(It.IsAny<string>()))
                .ReturnsAsync(movies);
            var result = await _movieBusiness.SearchMovie(It.IsAny<string>());
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetMovieByName_InValid_ReturnsFalse()
        {
            var movies = _fixture.Build<List<MovieStatus>>().Create();
            _movieRepositoryMock.Setup(m => m.SearchMovie(It.IsAny<string>()))
                .ReturnsAsync(movies);
            var result = await _movieBusiness.SearchMovie(It.IsAny<string>());
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddMovie_OnValidRequest_ReturnsTrue()
        {
            var request = _fixture.Build<MovieDto>().Create();
            var repoRequest = _fixture.Build<MovieStatus>().Create();
            _movieRepositoryMock.Setup(a => a.AddMovieRepoAsync(repoRequest));
            var result = await _movieBusiness.AddMovieAsync(request);
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task AddMovie_OnInValidRequest_ReturnsFalse()
        {
            var repoRequest = _fixture.Build<MovieStatus>().Create();
            _movieRepositoryMock.Setup(a => a.AddMovieRepoAsync(repoRequest));
            var result = await _movieBusiness.AddMovieAsync(null);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task DeleteMovie_OnValidRequest_ReturnsTrue()
        {
            var movie = new List<MovieStatus>
            {
                new MovieStatus
                {
                    MovieName = "Sita",
                    TheatreName = "PVR",
                    NumberOfTickets = 50,
                    TicketStatus = "BOOK ASAP"
                }
            };
            _movieRepositoryMock.Setup(a => a.GetMovieRepoAsync(It.IsAny<string>()))
                .ReturnsAsync(movie);
            _movieRepositoryMock.Setup(a => a.DeleteMovieRepoAsync(It.IsAny<string>()));
            var result = await _movieBusiness.DeleteMovieAsync(It.IsAny<string>());
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task DeleteMovie_OnInValidRequest_ReturnsTrue()
        {
            var request = _fixture.Build<MovieDto>().Create();
            var repoRequest = _fixture.Build<MovieStatus>().Create();
            _movieRepositoryMock.Setup(a => a.DeleteMovieRepoAsync(null));
            var result = await _movieBusiness.DeleteMovieAsync(null);
            Assert.That(result.Success, Is.False);
        }
    

}
}
