namespace HikeRent.DTOs.Brands
{
    public class BrandResponse
    {
        public int BrandId { get; set; }

        public string BrandName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
