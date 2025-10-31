namespace real_estate_api.Application.DTO
{
    public class PropertyDetailDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public string CodeInternal { get; set; } = null!;
        public int Year { get; set; }

        // Owner Information
        public string OwnerId { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string OwnerAddress { get; set; } = null!;
        public string? OwnerPhoto { get; set; }
        public DateTime? OwnerBirthday { get; set; }

        // Images
        public List<PropertyImageDto> Images { get; set; } = new List<PropertyImageDto>();

        // Traces
        public List<PropertyTraceDto> Traces { get; set; } = new List<PropertyTraceDto>();
    }
}
