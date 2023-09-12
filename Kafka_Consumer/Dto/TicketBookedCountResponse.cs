namespace Kafka_Consumer.Dto
{
    public class TicketBookedCountResponse
    {
        public int TicketCount { get; set; }
        public string TheatreName { get; set; }
        public string? Status { get; set; }
        public string ImageUrl { get; set; }
    }
}
