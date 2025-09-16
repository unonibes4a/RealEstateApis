using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstateApi.Models
{
    public class Property
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("idOwner")]
        public string IdOwner { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("codeInternal")]
        public string CodeInternal { get; set; } = string.Empty;

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("image")]
        public string Image { get; set; } = string.Empty;
    }
}