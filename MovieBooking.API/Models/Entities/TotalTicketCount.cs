using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieBooking.API.Models.Entities
{
    public class TotalTicketCount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string MovieName { get; set; }

        [BsonElement("Tickets")]
        public int NumberOfTickets { get; set; }

        [BsonElement("ImageURL")]
        public string ImageUrl { get; set; }
    }
}
