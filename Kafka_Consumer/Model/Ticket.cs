using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kafka_Consumer.Model
{
    public class Ticket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Movie")]
        public string MovieName { get; set; }

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
