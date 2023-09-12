using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MovieBooking.API.Models.Entities
{
    public class MovieStatus
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string MovieName { get; set; }

        [BsonElement("Theatre")]
        public string TheatreName { get; set; }

        [BsonElement("Tickets")]
        public int NumberOfTickets { get; set; }

        [BsonElement("Status")]
        public string TicketStatus { get; set; }

        [BsonElement("ImageURL")] 
        public string ImageUrl { get; set; } // Property for the image URL
    }
}
