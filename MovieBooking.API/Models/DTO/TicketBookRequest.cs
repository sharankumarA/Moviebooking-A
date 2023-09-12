using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MovieBooking.API.Models.DTO
{
    public class TicketBookRequest
    {

        [BsonElement("Theatre")]
        public string TheatreName { get; set; }

        [BsonElement("Tickets")]
        public int NumberOfTickets { get; set; }

        [BsonElement("SeatNumber")]
        public int[] SeatNumber { get; set; }

        [BsonElement("UserName")]
        public string UserName { get; set; }
    }
}
