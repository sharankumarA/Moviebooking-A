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
    public class TicketBusinessTest
    {
        private ITicketBusiness _ticketBusiness;
        private IFixture _fixture;
        private readonly Mock<ILogger<TicketBusiness>> _logger = new Mock<ILogger<TicketBusiness>>();
        private readonly Mock<ITicketRepository> _ticketRepositoryMock = new Mock<ITicketRepository>();

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _ticketBusiness = new TicketBusiness(_ticketRepositoryMock.Object, _logger.Object);
        }

        [Test]
        public async Task BookMovie_WhenMovieNotExists_ReturnsFalse()
        {
            var ticketRequest = _fixture.Build<TicketBookRequest>()
                .Create();
            _ticketRepositoryMock.Setup(t => t.CheckIfMovieExists(It.IsAny<string>()))
                 .ReturnsAsync(false);
            var result = await _ticketBusiness.BookMovieAsync(It.IsAny<string>(), ticketRequest);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task BookMovie_WhenTheatreNotExists_ReturnsFalse()
        {
            var ticketRequest = _fixture.Build<TicketBookRequest>()
                .Create();
            _ticketRepositoryMock.Setup(t => t.CheckIfMovieExists(It.IsAny<string>()))
                 .ReturnsAsync(true);
            _ticketRepositoryMock.Setup(t => t.GetMovieAgainstTheatreAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((MovieStatus)null);
            var result = await _ticketBusiness.BookMovieAsync(It.IsAny<string>(), ticketRequest);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task BookMovie_WhenSeatNoNotSameAsTicketNoExists_ReturnsFalse()
        {
            var ticketRequest = _fixture.Build<TicketBookRequest>()
                .With(t => t.NumberOfTickets, 2)
                .With(t => t.SeatNumber, new int[1])
                .Create();
            var movie = _fixture.Build<MovieStatus>().Create();
            _ticketRepositoryMock.Setup(t => t.CheckIfMovieExists(It.IsAny<string>()))
                 .ReturnsAsync(true);
            _ticketRepositoryMock.Setup(t => t.GetMovieAgainstTheatreAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(movie);
            var result = await _ticketBusiness.BookMovieAsync(It.IsAny<string>(), ticketRequest);
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task BookMovie_WhenValidRequest_ReturnsTrue()
        {
            var ticketRequest = _fixture.Build<TicketBookRequest>()
                .With(t => t.NumberOfTickets, 2)
                .With(t => t.SeatNumber, new int[2])
                .Create();
            var movie = _fixture.Build<MovieStatus>().Create();
            _ticketRepositoryMock.Setup(t => t.CheckIfMovieExists(It.IsAny<string>()))
                 .ReturnsAsync(true);
            _ticketRepositoryMock.Setup(t => t.GetMovieAgainstTheatreAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(movie);
            var result = await _ticketBusiness.BookMovieAsync(It.IsAny<string>(), ticketRequest);
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task TicketCount_WhenMovieAvailable_ReturnsBookAsap()
        {
            var ticketRequest = _fixture.Build<TicketBookRequest>()
                .With(t => t.NumberOfTickets, 2)
                .Create();
            var movie = _fixture.Build<MovieStatus>()
                .With(t => t.NumberOfTickets, 100)
                .Create();
            _ticketRepositoryMock.Setup(t => t.GetMovieAgainstTheatreAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(movie);
            await _ticketBusiness.TicketCountAsync(It.IsAny<string>(), ticketRequest);
            Assert.AreEqual(movie.TicketStatus, "BOOK ASAP");
        }

        [Test]
        public async Task TicketCount_WhenMovieNotAvailable_ReturnsSoldOut()
        {
            var ticketRequest = _fixture.Build<TicketBookRequest>()
                .With(t => t.NumberOfTickets, 2)
                .Create();
            var movie = _fixture.Build<MovieStatus>()
                .With(t => t.NumberOfTickets, 2)
                .Create();
            _ticketRepositoryMock.Setup(t => t.GetMovieAgainstTheatreAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(movie);
            await _ticketBusiness.TicketCountAsync(It.IsAny<string>(), ticketRequest);
            Assert.AreEqual(movie.TicketStatus, "SOLD OUT");
        }

    }
}
