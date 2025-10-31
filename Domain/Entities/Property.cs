using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace real_estate_api.Domain.Entities
{
    public class Property
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        [BsonElement("Name")]
        public string Name { get; set; } = null!;
        [BsonElement("Address")]
        public string Address { get; set; } = null!;
        [BsonElement("Price")]
        public decimal Price { get; set; }
        [BsonElement("CodeInternal")]
        public string CodeInternal { get; set; } = null!;
        [BsonElement("Year")]
        public int Year { get; set; }
        [BsonElement("Owner")]
        public Owner Owner { get; set; } = null!;
        [BsonElement("Images")]
        public List<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        [BsonElement("Traces")]
        public List<PropertyTrace> Traces { get; set; } = new List<PropertyTrace>();
    }
}
