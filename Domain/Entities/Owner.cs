using MongoDB.Bson.Serialization.Attributes;

namespace real_estate_api.Domain.Entities
{
    public class Owner
    {
        [BsonElement("OwnerId")]
        public string OwnerId { get; set; } = null!;

        [BsonElement("Name")]
        public string Name { get; set; } = null!;

        [BsonElement("Address")]
        public string Address { get; set; } = null!;

        [BsonElement("Photo")]
        public string Photo { get; set; } = null!;

        [BsonElement("Birthday")]
        public DateTime Birthday { get; set; }
    }
}