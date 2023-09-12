using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieBooking.API.Models.Entities
{
    public class TotalBookedMovies
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string MovieName { get; set; }

        [BsonElement("Tickets")]
        public int NumberOfTickets { get; set; }
        [BsonElement("Status")]
        public string TicketStatus { get; set; }

        [BsonElement("ImageURL")]
        public string ImageUrl { get; set; } // Property for the image URL
    }
}
