namespace real_estate_api.Application.DTO
{
    public class PropertyDto
    {
        public string Id { get; set; } = null!;
        public string IdOwner { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string PropertyName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Image { get; set; }
    }
}
