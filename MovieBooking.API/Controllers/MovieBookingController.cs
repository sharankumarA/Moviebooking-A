using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.API.Filters;
using MovieBooking.API.Interfaces.IBusiness;
using MovieBooking.API.Models.DTO;
using System.Web.Http.Cors;

namespace MovieBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MovieBookingController : ControllerBase
    {
        private readonly IUserBusiness _userBusiness;
        private readonly IMovieBusiness _movieBusiness;
        private readonly ITicketBusiness _ticketBusiness;
        private readonly ILogger<MovieBookingController> _logger;
       

        public MovieBookingController(IUserBusiness userBusiness, IMovieBusiness movieBusiness, ITicketBusiness ticketBusiness, ILogger<MovieBookingController> logger)
        {
            _userBusiness = userBusiness;
            _movieBusiness = movieBusiness;
            _ticketBusiness = ticketBusiness;
            _logger = logger;
        }

        

        [HttpPost("Register")]
        [ServiceFilter(typeof(NullCheckFilter))] 
       public async Task<ActionResult> Register(UserDto user)
        {
            _logger.LogInformation("Register user : MovieBooking Controller");
            _logger.LogDebug($"RegisterRequest : {user}");

            var userId = await _userBusiness.AddUser(user);

            if(!string.IsNullOrEmpty(userId))
            {
                return Ok(new
                {
                    Message = "Registered Successfully"
                });
            }
            else
            {
                return BadRequest("User already exists");
            }
        }

        [HttpGet("{loginId}/{password}")]
        public async Task<ActionResult<string>> Login([FromRoute] string loginId, [FromRoute] string password)
        {
            _logger.LogInformation("Login user : MovieBooking Controller");
            _logger.LogDebug($"LoginId : {loginId}, Password: {password}");

            var token = await _userBusiness.GetUserToken(loginId, password);

            if(!string.IsNullOrEmpty(token))
            {
                return Ok(token);
            }
            else
            {
                return BadRequest("wrong credentials");
            }
        }

        [HttpGet("forget/{loginId}/{newPassword}")]
        public async Task<ActionResult<string>> Forgot(string loginId, string newPassword)
        {
            _logger.LogInformation("Reset password : MovieBooking Controller");
            _logger.LogDebug($"LoginId : {loginId}, Reset password : {newPassword}");

            var passwordChangedStatus = await _userBusiness.ChangePassword(loginId, newPassword);

            if(!string.IsNullOrEmpty(passwordChangedStatus))
            {
                return Ok(passwordChangedStatus);
            }
            return BadRequest(passwordChangedStatus);

        }

        [HttpGet("All")]
        [Authorize(Roles ="users,admin")]
        public async Task<IActionResult> ViewAllMovies()
        {
            _logger.LogInformation("Get all movies from MovieBooking Controller");

            var movies = await _movieBusiness.GetMovies();
            
            if(movies is not null && movies.Count > 0)
            {
                return Ok(movies);
            }
            return NotFound("No Movies found");
        }

        [HttpGet("All/moviestatus")]
        [Authorize(Roles = "users,admin")]
        public async Task<IActionResult> ViewAllMovieStatus()
        {
            _logger.LogInformation("Get all movies from MovieBooking Controller");

            var movies = await _movieBusiness.GetMovieStatus();

            if (movies is not null && movies.Count > 0)
            {
                return Ok(movies);
            }
            return NotFound("No Movies found");
        }

        [HttpGet("Movies/Search/{movieName}")]
        [Authorize(Roles = "users")]
        public async Task<IActionResult> SearchMovie(string movieName)
        {
            _logger.LogInformation("Get movie by name from MovieBooking Controller");
            _logger.LogDebug($"moviename: {movieName}");

            var movies = await _movieBusiness.SearchMovie(movieName);

            if (movies is not null && movies.Count > 0)
            {
                return Ok(movies);
            }
            return NotFound("Movie doesn't exists");
        }

        [HttpPost]
        [Route("{moviename}/booktickets")]
        [Authorize(Roles ="users")]
        public async Task<IActionResult> BookTicket(string moviename, TicketBookRequest ticket)
        {
            _logger.LogInformation("Book ticket from MovieBooking Controller");
            _logger.LogDebug($"moviename: {moviename}, ticket: {ticket}");

            var result = await _ticketBusiness.BookMovieAsync(moviename, ticket);

            if(result.Success)
            {
               return Ok(ticket);
            }  

            return BadRequest(result.Message);
        }

        [HttpPost]
        [Route("add/movie")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddMovie(MovieDto movie)
        {
            _logger.LogInformation("Add movie : MovieBooking controller");
            _logger.LogDebug($"movie: {movie}");

            var result = await _movieBusiness.AddMovieAsync(movie);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }

        [HttpDelete]
        [Route("delete/{moviename}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteMovie(string moviename)
        {
            _logger.LogInformation("Delete movie from MovieBooking Controller");
            _logger.LogDebug($"moviename: {moviename}");

            var result = await _movieBusiness.DeleteMovieAsync(moviename);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }

        [HttpPut]
        [Route("{moviename}/update/{status}")]
        
        public async Task<IActionResult> UpdateTicketStatus(string moviename, string status)
        {
            _logger.LogInformation("Update ticket status : MovieBooking controller");
            _logger.LogDebug($"moviename: {moviename},MoviestatusUpdate: {status}");

            var result = await _movieBusiness.UpdateMovieStatus(moviename, status);
            if (result is not null)
            {
                return Ok(result);
            }

            return NotFound("Entered movie is not valid");
        }

        [HttpGet]
        [Route("cart/{username}")]
        [Authorize(Roles = "users")]
        public async Task<IActionResult> GetCartDetails(string username)
        {
            _logger.LogInformation("Get cart details:MovieBooking controller");
            _logger.LogDebug($"username: {username}");

            var result = await _ticketBusiness.CartDetails(username);
            if(result is not null)
            {
                return Ok(result);
            }

            return NotFound($"Cart not found for this {username}");
        }

        [HttpGet]
        [Route("theatrename/{moviename}")]
        
        public async Task<IActionResult> GettheatreNames(string moviename)
        {
            _logger.LogInformation("Get theatre details:MovieBooking controller");
            _logger.LogDebug($"movieName: {moviename}");

            var result = await _ticketBusiness.theatreName(moviename);
            if (result is not null)
            {
                return Ok((result));
            }

            return NotFound($"Theatre not found for this {moviename}");
        }

        [HttpGet]
        [Route("MoviestatusTicketcount/{moviename}/{theatreName}")]

        public async Task<IActionResult> GetAvailableMovieTickets(string moviename,string theatreName)
        {
            _logger.LogInformation("Get available movie tickets:MovieBooking controller");
            _logger.LogDebug($"movieName: {moviename}");

            var availableresult = await _movieBusiness.AvailableTicketCount(moviename, theatreName);
           
            if (availableresult != 0)
            {
                return Ok((availableresult));
            }
           

            return NotFound($"No available tickets {moviename}");

        }

        [HttpGet]
        [Route("Bookedseats/{moviename}/{theatreName}")]

        public async Task<IActionResult> GetTotalBookedSeatsCount(string moviename, string theatreName)
        {
            _logger.LogInformation("Get available movie tickets:MovieBooking controller");
            _logger.LogDebug($"movieName: {moviename}");

            var bookedresult = await _ticketBusiness.BookedTicketCount(moviename, theatreName);
            if ( bookedresult.Count> 0)
            {
                return Ok(( bookedresult));
            }
            return NotFound($"No booked tickets {moviename}");

        }

        [HttpGet]
        [Route("ticketcountandstatus/{moviename}/{theatreName}")]

        public async Task<IActionResult> GetTotalticketandstatus(string moviename, string theatreName)
        {
            _logger.LogInformation("Get movie tickets and status:MovieBooking controller");
            _logger.LogDebug($"movieName: {moviename}");
            _logger.LogDebug($"TheatreName: {theatreName}");

            var result = await _movieBusiness.TicketAndStatus(moviename, theatreName);

            if (result is not null)
            {
                return Ok((result));
            }
            return NotFound($"No booked tickets {moviename}");

        }
        [HttpPost]
        [Route("update/movie")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateMovie(InitialMovieTicketDto moviedata)
        {
            _logger.LogInformation("Update movie : MovieBooking controller");
          
            var result = await _movieBusiness.UpdateMovieAsync(moviedata);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result.Message);
        }

        [HttpGet("totalticketcount")]
        [Authorize(Roles = "users,admin")]
        public async Task<IActionResult> totalTicketCount()
        {
            _logger.LogInformation("Get all movies from MovieBooking Controller");

            var tickets = await _ticketBusiness.GetTickets();

            if (tickets is not null && tickets.Count > 0)
            {
                return Ok(tickets);
            }
            return NotFound("No Movies found");
        }




    }
}
