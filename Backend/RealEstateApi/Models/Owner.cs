using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstateApi.Models
{
    public class Owner
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("photo")]
        public string Photo { get; set; } = string.Empty;

        [BsonElement("birthday")]
        public DateTime Birthday { get; set; }
    }
}