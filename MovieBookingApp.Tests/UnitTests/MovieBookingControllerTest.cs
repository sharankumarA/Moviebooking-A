using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MovieBooking.API.Controllers;
using MovieBooking.API.Interfaces.IBusiness;
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
    public class MovieBookingControllerTest
    {
        private IMovieBusiness _movieBusiness;
        private ITicketBusiness _ticketBusiness;
        private IUserBusiness _userBusiness;
        private readonly Mock<IMovieBusiness> _movieServiceMock = new Mock<IMovieBusiness>();
        private readonly Mock<ITicketBusiness> _ticketServiceMock = new Mock<ITicketBusiness>();
        private readonly Mock<IUserBusiness> _userServiceMock = new Mock<IUserBusiness>();
        private readonly Mock<ILogger<MovieBookingController>> _logger = new Mock<ILogger<MovieBookingController>>();
        private IFixture _fixture;
        private MovieBookingController _movieBookingController;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _movieBusiness = _movieServiceMock.Object;
            _ticketBusiness = _ticketServiceMock.Object;
            _userBusiness = _userServiceMock.Object;
            _movieBookingController = new MovieBookingController(_userBusiness, _movieBusiness, _ticketBusiness, _logger.Object);
        }

        //Movie- GetAllMovies and GetSingleMovieByname

        [Test]
        public async Task GetMovies_Valid_ReturnsOk()
        {
            var movies = new List<MovieDto>
            {
                new MovieDto
                {
                    Name = "VIP",
                    TheatreName = "PVR",
                    NumberOfTickets = 50,
                    TicketStatus = "BOOK ASAP"
                }
            };
            _movieServiceMock.Setup(m => m.GetMovies())
                .ReturnsAsync(movies);

            var result = await _movieBookingController.ViewAllMovies();
            var okResult = result as OkObjectResult;

            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetMovies_InValid_ReturnsNotFoundResponse()
        {
            _movieServiceMock.Setup(m => m.GetMovies())
                .ReturnsAsync((List<MovieDto>)null);

            var result = await _movieBookingController.ViewAllMovies();
            var notfoundResult = result as NotFoundObjectResult;

            Assert.AreEqual(404, notfoundResult.StatusCode);
        }

        [Test]
        public async Task GetMoviesByName_Valid_ReturnsOk()
        {
            var movies = new List<MovieDto>
            {
                new MovieDto
                {
                    TheatreName = "PVR",
                    Name = "Sita",
                    NumberOfTickets = 50,
                    TicketStatus = "BOOK ASAP"
                }
            };
            _movieServiceMock.Setup(m => m.SearchMovie(It.IsAny<string>()))
                .ReturnsAsync(movies);

            var result = await _movieBookingController.SearchMovie(It.IsAny<string>());
            var okResult = result as OkObjectResult;

            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task GetMoviesByName_InValid_ReturnsNotFoundResponse()
        {
            _movieServiceMock.Setup(m => m.SearchMovie(It.IsAny<string>()))
                .ReturnsAsync((List<MovieDto>)null);

            var result = await _movieBookingController.SearchMovie(It.IsAny<string>());
            var notfoundResult = result as NotFoundObjectResult;

            Assert.AreEqual(404, notfoundResult.StatusCode);
        }



        //BookTicket

        [Test]
        public async Task BookTicket_Valid_ReturnsOk()
        {
            var request = _fixture.Build<TicketBookRequest>().Create();
            var response = _fixture.Build<TicketBookingResponse>()
                .With(t => t.Success, true)
                .Create();
            _ticketServiceMock.Setup(m => m.BookMovieAsync(It.IsAny<string>(), request))
                .ReturnsAsync(response);

            var result = await _movieBookingController.BookTicket(It.IsAny<string>(), request);
            var okResult = result as OkObjectResult;

            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task BookTicket_InValid_ReturnsBadResponse()
        {
            var request = _fixture.Build<TicketBookRequest>().Create();
            var response = _fixture.Build<TicketBookingResponse>()
                .With(t => t.Success, false)
                .Create();
            _ticketServiceMock.Setup(m => m.BookMovieAsync(It.IsAny<string>(), request))
                .ReturnsAsync(response);

            var result = await _movieBookingController.BookTicket(It.IsAny<string>(), request);
            var badResult = result as BadRequestObjectResult;

            Assert.AreEqual(400, badResult.StatusCode);
        }

        //Add Movies

        [Test]
        public async Task AddMovie_WhenValidInput_ReturnsTrue()
        {
            var movie = _fixture.Build<MovieDto>().Create();
            var response = _fixture.Build<MovieResponse>().Create();
            _movieServiceMock.Setup(a => a.AddMovieAsync(movie))
                .ReturnsAsync(response);

            var result = await _movieBookingController.AddMovie(movie);
            var okresult = result as OkObjectResult;

            Assert.AreEqual(200, okresult.StatusCode);
        }

        [Test]
        public async Task AddMovie_WhenInValidInput_ReturnsFalse()
        {
            var movie = _fixture.Build<MovieDto>().Create();
            var response = _fixture.Build<MovieResponse>()
                .With(t => t.Success, false).Create();
            _movieServiceMock.Setup(a => a.AddMovieAsync(null))
                .ReturnsAsync(response);

            var result = await _movieBookingController.AddMovie(null);
            var badresult = result as BadRequestObjectResult;

            Assert.AreEqual(400, badresult.StatusCode);
        }

        // Delete Movies


        [Test]
        public async Task DeleteMovie_WhenValidInput_ReturnsTrue()
        {
            var movie = _fixture.Build<MovieDto>().Create();
            var response = _fixture.Build<MovieResponse>().Create();
            _movieServiceMock.Setup(a => a.DeleteMovieAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            var result = await _movieBookingController.DeleteMovie(It.IsAny<string>());
            var okresult = result as OkObjectResult;

            Assert.AreEqual(200, okresult.StatusCode);
        }

        [Test]
        public async Task DeleteMovie_WhenInValidInput_ReturnsFalse()
        {
            var movie = _fixture.Build<MovieDto>().Create();
            var response = _fixture.Build<MovieResponse>()
                .With(t => t.Success, false).Create();
            _movieServiceMock.Setup(a => a.DeleteMovieAsync(null))
                .ReturnsAsync(response);

            var result = await _movieBookingController.DeleteMovie(null);
            var badresult = result as BadRequestObjectResult;

            Assert.AreEqual(400, badresult.StatusCode);
        }

        //Update Ticketstatus
        [Test]
        public async Task UpdateTicketStatus_WhenValidInput_ReturnsTrue()
        {
            var movie = _fixture.Build<MovieDto>().Create();
            var response = _fixture.Build<MovieStatus>().Create();
            _movieServiceMock.Setup(a => a.UpdateMovieStatus(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            var result = await _movieBookingController.UpdateTicketStatus(It.IsAny<string>(), It.IsAny<string>());
            var okresult = result as OkObjectResult;

            Assert.AreEqual(200, okresult.StatusCode);
        }

        

    }
}
