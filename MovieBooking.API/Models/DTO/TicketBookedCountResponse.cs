namespace MovieBooking.API.Models.DTO
{
    public class TicketBookedCountResponse
    {
        public int TicketCount { get; set; }
        public string TheatreName { get; set; }
        public string? Status { get; set; }
        public string ImageUrl { get; set; }
    }
}
