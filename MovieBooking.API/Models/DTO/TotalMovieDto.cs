namespace MovieBooking.API.Models.DTO
{
    public class TotalMovieDto
    {
        public string Name { get; set; } = string.Empty;
        public int NumberOfTickets { get; set; }
        public string TicketStatus { get; set; }

        public string ImageUrl { get; set; }
    }
}
