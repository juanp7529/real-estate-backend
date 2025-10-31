using MongoDB.Bson.Serialization.Attributes;

namespace real_estate_api.Domain.Entities
{
    public class PropertyImage
    {
        [BsonElement("ImageId")]
        public string ImageId { get; set; } = null!;
        [BsonElement("File")]
        public string File { get; set; } = null!;
        [BsonElement("Enabled")]
        public bool Enabled { get; set; }
    }
}
