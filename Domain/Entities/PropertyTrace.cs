using MongoDB.Bson.Serialization.Attributes;

namespace real_estate_api.Domain.Entities
{
    public class PropertyTrace
    {
        [BsonElement("TraceId")]
        public string TraceId { get; set; } = null!;
        [BsonElement("Name")]
        public string Name { get; set; } = null!;
        [BsonElement("DateSale")]
        public DateTime DateSale { get; set; }
        [BsonElement("Value")]
        public decimal Value { get; set; }
        [BsonElement("Tax")]
        public decimal Tax { get; set; }
    }
}
