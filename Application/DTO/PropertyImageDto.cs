namespace real_estate_api.Application.DTO
{
    public class PropertyImageDto
    {
        public string Id { get; set; } = null!;
        public string File { get; set; } = null!;
        public bool Enabled { get; set; }
    }
}
