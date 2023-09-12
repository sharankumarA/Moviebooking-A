using MovieBooking.API.Interfaces.IBusiness;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models.DTO;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Business
{
    public class TicketBusiness:ITicketBusiness
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<TicketBusiness> _logger;

        private Dictionary<string, int> seat = new Dictionary<string, int>();

        public TicketBusiness(ITicketRepository ticketRepository, ILogger<TicketBusiness> logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }



        public async Task<TicketBookingResponse> BookMovieAsync(string moviename, TicketBookRequest ticket)
        {
            _logger.LogInformation("Book movie : ticket business");

            var movie = await _ticketRepository.CheckIfMovieExists(moviename);
            if (movie)
            {
                var theatre = await _ticketRepository.GetMovieAgainstTheatreAsync(ticket.TheatreName, moviename);
                if (theatre != null)
                {
                    if (ticket.NumberOfTickets == ticket.SeatNumber.Length)
                    {
                        if (CheckIfSeatExists(ticket))
                        {
                            var ticketInsert = new Ticket
                            {
                                UserName = ticket.UserName,
                                MovieName = moviename,
                                TheatreName = ticket.TheatreName,
                                NumberOfTickets = ticket.NumberOfTickets,
                                SeatNumber = ticket.SeatNumber
                            };
                            await _ticketRepository.BookMovieRepoAsync(ticketInsert);
                            await TicketCountAsync(moviename, ticket);
                            await TotalTicketCountAsync(moviename, ticket);
                            return new TicketBookingResponse
                            {
                                Message = "Ticket booked successfully",
                                Success = true,
                                UserName = ticket.UserName
                            };
                        }
                        else
                        {
                            return new TicketBookingResponse
                            {
                                Message = "Seat number is taken. Try another seat number",
                                Success = false
                            };
                        }
                    }
                    else
                    {
                        return new TicketBookingResponse
                        {
                            Message = "Number of seats is not same as number of tickets",
                            Success = false
                        };
                    }
                }
                else
                {
                    return new TicketBookingResponse
                    {
                        Message = "Theatre not found",
                        Success = false
                    };
                }
            }
            else
            {
                return new TicketBookingResponse
                {
                    Message = "Movie not found",
                    Success = false
                };
            }
        }

       public async Task TicketCountAsync(string moviename, TicketBookRequest ticket)
        {
          
            var movie = await _ticketRepository.GetMovieAgainstTheatreAsync(ticket.TheatreName, moviename);
            var ticketCount = ticket.NumberOfTickets;
            movie.NumberOfTickets -= ticketCount;
            if (movie.NumberOfTickets > 0)
            {
               var result= movie.TicketStatus = "BOOK ASAP";
                await _ticketRepository.UpdateTicketCount(movie);
               

            }
            else
            {
                var result=movie.TicketStatus = "SOLD OUT";
                await _ticketRepository.UpdateTicketCount(movie);
               

            }
            
        }

        public async Task TotalTicketCountAsync(string moviename, TicketBookRequest ticket)
        {

            var totalMovie = await _ticketRepository.GetTotalTicketsAsync(moviename);
            var ticketCount = ticket.NumberOfTickets;
            totalMovie.NumberOfTickets -= ticketCount;
            if (totalMovie.NumberOfTickets > 0)
            {
                var result = totalMovie.TicketStatus = "BOOK ASAP";
                await _ticketRepository.UpdateTotalTicketCount(totalMovie);


            }
            else
            {
                var result = totalMovie.TicketStatus = "SOLD OUT";
                await _ticketRepository.UpdateTotalTicketCount(totalMovie);


            }

        }

        public bool CheckIfSeatExists(TicketBookRequest ticket)
         {
             var theatre = ticket.TheatreName;

             foreach (var seatNo in ticket.SeatNumber)
             {
                 if (seat.ContainsKey(theatre) && seat.ContainsValue(seatNo))
                 {
                     return false;
                 }
                 else
                 {
                     seat.Add(theatre, seatNo);
                     return true;
                 }
             }
             return true;
         }

        public async Task<List<TicketDto>> CartDetails(string username)
        {
            _logger.LogInformation("cart details : ticket business");

            List<TicketDto> cartdetails;

            try
            {
                var ticketlist= new List<TicketDto>();
                var ticketdetails= await _ticketRepository.getcartdetails(username);
                if (ticketdetails.Count > 0)
                {
                    foreach (var ticket in ticketdetails)
                    {
                        var ticketResponse =
                            new TicketDto
                            {
                                MovieName = ticket.MovieName,
                                TheatreName = ticket.TheatreName,
                                NumberOfTickets = ticket.NumberOfTickets,
                                SeatNumber = ticket.SeatNumber,

                            };
                        ticketlist.Add(ticketResponse);
                    }
                    return ticketlist;
                }
                return null;
            }
            catch (Exception)
            {
                cartdetails = new();
            }
            return cartdetails;
        }
        public async Task<List<string>> theatreName(string moviename)
        {
            var theatreList = new List<string>();

            var theatrenames = await _ticketRepository.GetTheatreName(moviename);
            if (theatrenames.Count > 0)
            {
                var theatreNames = theatrenames.Select(m => m.TheatreName).Distinct();

                theatreList.AddRange(theatreNames);
            }
            return theatreList;
        }
        public async Task<List<int>> BookedTicketCount(string moviename, string theatreName)
        {
            var bookedTicketCount = await _ticketRepository.bookedcount(moviename, theatreName);

            if(bookedTicketCount.Count>0)
            {
                return bookedTicketCount;
            }
            return new List<int>();
        }

        public async Task<List<TotalTicketCount>> GetTickets()
        {
            var allticketcount = await _ticketRepository.getallticketcount();

            if(allticketcount is not null)
            {
                return allticketcount;
            }
            else
            {
                return null;
            }
        }
    }
}