namespace MovieBooking.API.Models.DTO
{
    public class TicketDto
    {
        public string MovieName { get; set; }
        public string TheatreName { get; set; }
        public int NumberOfTickets { get; set; }
        public int[] SeatNumber { get; set; }
    }
}
