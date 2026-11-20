namespace HikeRent.DTOs.GearItems
{
    public class GearItemResponse
    {
        public int GearItemId { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int BrandId { get; set; }

        public string BrandName { get; set; } = string.Empty;

        public string GearName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal DailyPrice { get; set; }

        public decimal ReplacementPrice { get; set; }

        public int TotalStock { get; set; }

        public int MaintenanceStock { get; set; }

        public int DamagedStock { get; set; }
        public int AvailableStock { get; set; }

        public bool IsActive { get; set; }

        public string? ImageUrl { get; set; }
    }
}
