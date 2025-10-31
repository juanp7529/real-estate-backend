namespace real_estate_api.Application.DTO
{
    public class PropertyTraceDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime DateSale { get; set; }
        public decimal Value { get; set; }
        public decimal Tax { get; set; }
    }
}
